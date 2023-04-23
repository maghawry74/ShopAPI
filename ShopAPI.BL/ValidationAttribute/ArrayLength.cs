using ShopAPI.BL.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ShopAPI.BL;

public class ArrayLength : ValidationAttribute
{
    int ValidLength;
    public ArrayLength(int ValidLength)
    {
        this.ValidLength = ValidLength;
    }
    public override bool IsValid(object? value)
    {
        var Arr = value as IEnumerable<OrderProductDTO>;
        if (Arr == null || Arr.Count() < ValidLength)
        {
            return false;
        }
        return true;
    }
}

