using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.DataAccess.FormData_
{
    public class QueryFactory
    {
        //private static string CREATE_TABLE_FILENAME = "CreateFormTable.txt";
        //private static string INSERT_ROW_FILENAME = "InsertRow.txt";
        //private static string UPDATE_ROW_FILENAME = "UpdateRow.txt";
        //private static string GET_DATA_FILENAME = "GetTableData.txt";
        //private static string COUNT_DATA_FILENAME = "CountTableData.txt";
        //public static string BuildScript(DynamicQueryTypes queryType, DynamicTable entity)
        //{
        //    if (queryType == DynamicQueryTypes.CreateTable)
        //    {
        //        return _GetResourceFileContentAsString(CREATE_TABLE_FILENAME, entity);
        //    }
        //    else if (queryType == DynamicQueryTypes.InsertRow)
        //    {
        //        return _GetResourceFileContentAsString(CREATE_TABLE_FILENAME, entity);
        //    }
        //    else if (queryType == DynamicQueryTypes.UpdateRow)
        //    {
        //        return _GetResourceFileContentAsString(CREATE_TABLE_FILENAME, entity);
        //    }
        //    else if (queryType == DynamicQueryTypes.GetData)
        //    {
        //        return _GetResourceFileContentAsString(GET_DATA_FILENAME, entity);
        //    }
        //    else if (queryType == DynamicQueryTypes.GetDataCount)
        //    {
        //        return _GetResourceFileContentAsString(COUNT_DATA_FILENAME, entity);
        //    }
        //    return null;
        //}

        //private static string _GetResourceFileContentAsString(string fileName, DynamicTable entity)
        //{
        //    string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates", fileName);
        //    var query = File.ReadAllText(path);
        //    query = query.Replace(DynamicTableConstants.TABLE_NAME, entity.TableName ?? string.Empty)
        //        .Replace(DynamicTableConstants.TYPING_PROCES_ID, entity.TypingProcessId ?? string.Empty)
        //        .Replace(DynamicTableConstants.FORM_ID, entity.FormId.ToString())
        //        .Replace(DynamicTableConstants.REGISTER_TYPE, entity.RegisterType.Description() ?? string.Empty)
        //        .Replace(DynamicTableConstants.COMPLETED_SECTIONS, entity.CompletedSections.ToString() ?? string.Empty)
        //        .Replace(DynamicTableConstants.TEMPORAL_DATA, entity.TemporalData ?? string.Empty)
        //        .Replace(DynamicTableConstants.CREATED_BY, entity.CreatedBy ?? string.Empty)
        //        .Replace(DynamicTableConstants.CREATED_ON, entity.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss "))
        //        .Replace(DynamicTableConstants.MODIFIED_BY, entity.ModifiedBy ?? string.Empty)
        //        .Replace(DynamicTableConstants.MODIFIED_ON, entity.ModifiedOn.HasValue ? entity.ModifiedOn.Value.ToString("yyyy-MM-dd HH:mm:ss ") : string.Empty)
        //        .Replace(DynamicTableConstants.ORDER_COLUMN, entity.OrderColumn ?? string.Empty)
        //        .Replace(DynamicTableConstants.ORDER_DIRECTION, entity.OrderDirection == default(OrderDirections) ? "0" : entity.OrderDirection.Description())
        //        ;
        //    return query;
        //}
    }
}
