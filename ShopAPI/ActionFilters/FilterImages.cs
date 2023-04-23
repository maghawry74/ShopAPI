using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopAPI.BL.DTOs;

namespace ShopAPI.ActionFilters
{
    public class FilterImages : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var product = context.ActionArguments["newProduct"] as NewProductDTO;
            if (product != null)
            {
                string[] Types = { "image/jpeg", "image/png" };
                Console.WriteLine(product.ImageFile.ContentType);
                if (!Types.Contains(product.ImageFile.ContentType))
                {
                    context.ModelState.AddModelError("ImageFile", "Only JPG/JPEG/PNG Are Allowed");
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
                if (product.ImageFile.Length > 1_000_000)
                {
                    context.ModelState.AddModelError("ImageFile", "Max Size is 1 MegaByte");
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }
        }
    }
}
