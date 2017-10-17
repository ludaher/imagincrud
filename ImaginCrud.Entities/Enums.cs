using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Entities
{
    /// <summary>
    /// Funciones de un usuario en el sistema
    /// </summary>
    public enum UserFunctions
    {
        [Description("Digitador")]
        Typist = 1,
        [Description("Validador")]
        Validator = 2,
    }
    /// <summary>
    /// Posibles estados de un proceso
    /// </summary>
    public enum ProcessStatus
    {
        [Description("Pendiente")]
        Pending = 1,
        [Description("En captura")]
        InCapture = 2,
        [Description("Capturado")]
        Captured = 3,
        [Description("En validación")]
        Validating = 4,
        [Description("Validado")]
        Validated = 5,
    }
    /// <summary>
    /// Posibles estados de un producto
    /// </summary>
    public enum ProductStatus
    {
        [Description("Registrado")]
        Registered = 1,
        [Description("Recibido")]
        Received = 2,
        [Description("En captura")]
        InCapture = 3,
        [Description("Validado")]
        Valid = 4,
        [Description("Exportado")]
        Exported = 5
    }
    /// <summary>
    /// Tipos de istas de datos
    /// </summary>
    public enum DefaultDataLists
    {
        [Description("Personalizado")]
        Custom = 0
    }
    /// <summary>
    /// Tipos de captura en los processos
    /// </summary>
    public enum RegisterTypes
    {
        [Description("Captura")]
        Capture = 1,
        [Description("Captura completa")]
        CaptureComplete = 2,
        [Description("Validación")]
        Validation = 3,
        [Description("Validación completa")]
        ValidationComplete = 4,
    }

    public enum DynamicQueryTypes
    {
        CreateTable = 1,
        InsertRow = 2,
        UpdateRow = 3,
        GetData = 4,
        GetDataCount = 5,
    }
    /// <summary>
    /// Tipos de campos en un formulario
    /// </summary>
    public enum FieldTypes
    {
        [Description("Numero")]
        Number = 1,
        [Description("Texto")]
        Text = 2,
        [Description("Checkbox")]
        Checkbox = 3,
        [Description("Selección")]
        Select = 4,
        [Description("Selección múltiple")]
        MultiSelect = 5,
        [Description("Fecha")]
        DateTime = 6,
        [Description("Nueva fila")]
        NewRow = 7,
    }
    /// <summary>
    /// Prioridad de procesos
    /// </summary>
    public enum ProcessPriorities
    {
        [Description("Urgente")]
        Urgent = 1,
        [Description("Alto")]
        High = 2,
        [Description("Medio")]
        Half = 3,
        [Description("Bajo")]
        Low = 4
    }
}
