using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShopAPI.ActionFilters
{
    public class AllowedQueries : ActionFilterAttribute
    {
        string[] Allowed;
        string Query;
        public AllowedQueries(string Query, params string[] Allowed)
        {
            this.Allowed = Allowed;
            this.Query = Query;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var QueryValue = context.HttpContext.Request.Query.FirstOrDefault(Q => Q.Key.ToLower() == Query).Value[0];

            if (!Allowed.Contains(QueryValue))
            {
                context.Result = new BadRequestObjectResult($"{QueryValue} Isn't Valid");
            }
        }
    }
}
