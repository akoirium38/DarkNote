"""Convert Lucide SVG icons into a WPF ResourceDictionary of Geometry resources.

Usage: python -I lucide_to_xaml.py <svg_dir> <out.xaml>
Each icon becomes <Geometry x:Key="Icon.<PascalName>">...</Geometry> (24x24 canvas, stroke-only).
"""
import re
import sys
import xml.etree.ElementTree as ET

NUM = r"[-+]?(?:\d+\.?\d*|\.\d+)(?:[eE][-+]?\d+)?"
from pathlib import Path


def f(v):
    v = float(v)
    return str(int(v)) if v.is_integer() else f"{v:g}"


def circle(cx, cy, rx, ry):
    cx, cy, rx, ry = map(float, (cx, cy, rx, ry))
    return (f"M{f(cx - rx)},{f(cy)} A{f(rx)},{f(ry)} 0 1 0 {f(cx + rx)},{f(cy)} "
            f"A{f(rx)},{f(ry)} 0 1 0 {f(cx - rx)},{f(cy)} Z")


def rect(a):
    x, y = float(a.get("x", 0)), float(a.get("y", 0))
    w, h = float(a["width"]), float(a["height"])
    rx = float(a.get("rx", a.get("ry", 0)))
    ry = float(a.get("ry", rx))
    if rx == 0:
        return f"M{f(x)},{f(y)} H{f(x + w)} V{f(y + h)} H{f(x)} Z"
    return (f"M{f(x + rx)},{f(y)} H{f(x + w - rx)} A{f(rx)},{f(ry)} 0 0 1 {f(x + w)},{f(y + ry)} "
            f"V{f(y + h - ry)} A{f(rx)},{f(ry)} 0 0 1 {f(x + w - rx)},{f(y + h)} "
            f"H{f(x + rx)} A{f(rx)},{f(ry)} 0 0 1 {f(x)},{f(y + h - ry)} "
            f"V{f(y + ry)} A{f(rx)},{f(ry)} 0 0 1 {f(x + rx)},{f(y)} Z")


def points(p, close):
    nums = p.replace(",", " ").split()
    pairs = [f"{f(nums[i])},{f(nums[i + 1])}" for i in range(0, len(nums), 2)]
    return "M" + " L".join(pairs) + (" Z" if close else "")


def convert(el):
    tag = el.tag.split("}")[-1]
    a = el.attrib
    if tag == "path":
        # Each SVG <path> starts fresh, so its first "m" is absolute; once paths are
        # joined into one geometry a leading lowercase "m" would become relative.
        # Implicit pairs after "m" are relative line-tos, so "m x y rest" -> "M x y l rest".
        d = a["d"].strip()
        if not d.startswith("m"):
            return d
        m = re.match(r"m\s*(" + NUM + r")[\s,]*(" + NUM + r")\s*,?\s*(.*)", d, re.S)
        x, y, rest = m.groups()
        if rest and not rest[0].isalpha():
            rest = "l" + rest
        return f"M{x} {y} {rest}".strip()
    if tag == "circle":
        return circle(a["cx"], a["cy"], a["r"], a["r"])
    if tag == "ellipse":
        return circle(a["cx"], a["cy"], a["rx"], a["ry"])
    if tag == "rect":
        return rect(a)
    if tag == "line":
        return f"M{f(a['x1'])},{f(a['y1'])} L{f(a['x2'])},{f(a['y2'])}"
    if tag == "polyline":
        return points(a["points"], False)
    if tag == "polygon":
        return points(a["points"], True)
    raise ValueError(f"Unsupported SVG element: {tag}")


def pascal(name):
    return "".join(part.capitalize() for part in name.split("-"))


def main(svg_dir, out):
    lines = [
        '<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"',
        '                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">',
        "    <!-- Generated from lucide-static (ISC license, see Resources/Icons/LICENSE). 24x24 canvas,",
        "         draw with a Path: Stroke = text color, StrokeThickness = 2, round caps and joins, no Fill. -->",
    ]
    for svg in sorted(Path(svg_dir).glob("*.svg")):
        root = ET.parse(svg).getroot()
        data = " ".join(convert(el) for el in root)
        lines.append(f'    <Geometry x:Key="Icon.{pascal(svg.stem)}">{data}</Geometry>')
    lines.append("</ResourceDictionary>")
    Path(out).write_text("\n".join(lines) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main(sys.argv[1], sys.argv[2])
