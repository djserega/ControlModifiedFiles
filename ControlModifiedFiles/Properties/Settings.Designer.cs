﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControlModifiedFiles.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DirectoryCache {
            get {
                return ((string)(this["DirectoryCache"]));
            }
            set {
                this["DirectoryCache"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Autoload {
            get {
                return ((bool)(this["Autoload"]));
            }
            set {
                this["Autoload"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoupdateVersion {
            get {
                return ((bool)(this["AutoupdateVersion"]));
            }
            set {
                this["AutoupdateVersion"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>Поддерживаемые файлы (epf, erf, ert)|*.epf;*.erf;*.ert</string>
  <string>Внешняя обработка (epf)|*.epf</string>
  <string>Внешний отчет (erf)|*.erf</string>
  <string>Внешняя обработка 7.7 (ert)|*.ert</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection ListFilterFilesPredefined {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ListFilterFilesPredefined"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection ListFilterFiles {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ListFilterFiles"]));
            }
            set {
                this["ListFilterFiles"] = value;
            }
        }
    }
}
