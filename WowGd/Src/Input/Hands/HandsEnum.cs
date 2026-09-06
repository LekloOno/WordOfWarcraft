using System;

namespace WowGd.Src.Input.Hands;

[Flags]
public enum HandsEnum
{
    None    = 0,
    First   = 1,
    Second  = 1 << 1,
}