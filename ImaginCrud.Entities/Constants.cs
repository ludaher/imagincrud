using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Entities
{
    /// <summary>
    /// Constantes utilizadas en la aplicación
    /// </summary>
    public static class Constants
    {
        public const string SECURITY_ROLE = "Seguridad";
        public const string ADMINISTRATOR_ROLE = "Administrador";
        public const string TYPIST_ROLE = "Digitador";
        public const string VALIDATOR_ROLE = "Validador";
        public const string SUPERVISOR_ROLE = "Supervisor";
        public const string PROCESSES_LOCAL_PATH = "PROCESSES_LOCAL_PATH";
    }
    /// <summary>
    /// Constantes para queries
    /// </summary>
    public static class DynamicTableConstants
    {
        public static string TABLE_NAME = "{{TABLE_NAME}}";
        public static string TYPING_PROCES_ID = "{{TYPING_PROCES_ID}}";
        public static string FORM_ID = "{{FORM_ID}}";
        public static string REGISTER_TYPE = "{{REGISTER_TYPE}}";
        public static string COMPLETED_SECTIONS = "{{COMPLETED_SECTIONS}}";
        public static string TEMPORAL_DATA = "{{TEMPORAL_DATA}}";
        public static string CREATED_BY = "{{CREATED_BY}}";
        public static string CREATED_ON = "{{CREATED_ON}}";
        public static string MODIFIED_BY = "{{MODIFIED_BY}}";
        public static string MODIFIED_ON = "{{MODIFIED_ON}}";
        public static string TABLE_ID = "{{TABLE_ID}}";
        public static string ORDER_COLUMN = "{{ORDER_COLUMN}}";
        public static string ORDER_DIRECTION = "{{ORDER_DIRECTION }}";
    }
}
