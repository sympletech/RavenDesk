using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RavenDesk.MVC.Helpers
{
    public class ListToSelectList
    {
        public static List<SelectListItem> ConvertToSelectList<T>(IEnumerable<T> baseList, string valueFiled, string textField) where T : class
        {
            return ConvertToSelectList<T>(baseList, valueFiled, textField, false, "");
        }

        public static List<SelectListItem> ConvertToSelectList<T>(IEnumerable<T> baseList, string valueFiled, string textField, bool includeFirstEntrySelect, string firstEntrySelectText) where T : class
        {
            var result = new List<SelectListItem>();

            Type ty = typeof(T);
            if (baseList != null)
            {
                foreach (var item in baseList)
                {
                    var valprop = ty.GetProperty(valueFiled);
                    var val = valprop != null ? valprop.GetValue(item, null) : "";
                    var txtprop = ty.GetProperty(textField);
                    var txt = txtprop != null ? txtprop.GetValue(item, null) : "";

                    result.Add(new SelectListItem
                        {
                            Text = txt != null ? txt.ToString() : "",
                            Value = val != null ? val.ToString() : ""
                        });
                }
            }

            result = result.Distinct().OrderBy(x => x.Text).ToList();

            if (includeFirstEntrySelect)
            {
                result.Insert(0, new SelectListItem
                {
                    Text = firstEntrySelectText,
                    Value = ""
                });

            }

            return result;
        }

        public static List<SelectListItem> ConvertToSelectList(Hashtable baseTable, bool includeFirstEntrySelect, string firstEntrySelectText)
        {
            List<SelectListItem> Result = new List<SelectListItem>();

            foreach (DictionaryEntry item in baseTable)
            {
                Result.Add(new SelectListItem
                {
                    Text = item.Value.ToString(),
                    Value = item.Key.ToString()
                });

            }

            Result = Result.OrderBy(x => x.Text).ToList();

            if (includeFirstEntrySelect)
            {
                Result.Insert(0, new SelectListItem
                {
                    Text = firstEntrySelectText,
                    Value = ""
                });

            }

            return Result;
        }

        public static List<SelectListItem> ConvertToSelectList(IEnumerable<object> baseList)
        {
            var results = new List<SelectListItem>();
            foreach (var itm in baseList)
            {
                results.Add(new SelectListItem { 
                    Text = itm.ToString(),
                    Value = itm.ToString()
                });
            }

            return results;
        }
    }
}