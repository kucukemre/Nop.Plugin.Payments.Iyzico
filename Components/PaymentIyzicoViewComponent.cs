using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Iyzico.Components
{
    [ViewComponent(Name = "PaymentIyzico")]
    public class PaymentIyzicoViewComponent : NopViewComponent
    {        
        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            //prepare payment model
            var model = new PaymentInfoModel();

            //prepare years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //prepare months
            for (var i = 1; i <= 12; i++)
            {
                model.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            return View("~/Plugins/Payments.Iyzico/Views/PaymentInfo.cshtml", model);
        }
    }
}
