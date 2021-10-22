using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace APBox.Control
{
    public static class Helpers
    {

        #region Views

        public static MvcHtmlString FileEditorFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var htmlAttributes = new Dictionary<string, object>();

            htmlAttributes.Add("type", "file");
            htmlAttributes.Add("class", "btn btn-default");
            return helper.TextBoxFor(expression, null, htmlAttributes);
        }

        #endregion

        #region Enums

        public static MvcHtmlString EnumDropDownListSpecialFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool soloLectura = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem
                                                {
                                                    Text = GetEnumDescription(value),
                                                    Value = value.ToString(),
                                                    Selected = value.Equals(metadata.Model)
                                                };

            if (metadata.IsNullableValueType)
                items = SingleEmptyItem.Concat(items).OrderBy(a => a.Value);



            if (soloLectura)
            {
                var htmlAttributes = new { @class = "form-control", @disabled = "disabled" };
                return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
            }
            else
            {
                var htmlAttributes = new { @class = "form-control" };
                return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
            }
        }

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }

        #endregion

    }
}