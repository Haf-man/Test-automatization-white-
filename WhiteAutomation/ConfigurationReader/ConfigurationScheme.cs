﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.34209
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 
namespace ConfigurationReader {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class Configuration {
        
        private ControlConfigurationsControlConfiguration[] includedControlsField;
        
        private ControlConfigurationsControlConfiguration[] excludedControlsField;
        
        private string[] includedPropertiesField;
        
        private string[] excludedPropertiesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ControlConfiguration", IsNullable=false)]
        public ControlConfigurationsControlConfiguration[] IncludedControls {
            get {
                return this.includedControlsField;
            }
            set {
                this.includedControlsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ControlConfiguration", IsNullable=false)]
        public ControlConfigurationsControlConfiguration[] ExcludedControls {
            get {
                return this.excludedControlsField;
            }
            set {
                this.excludedControlsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("PropertieName", IsNullable=false)]
        public string[] IncludedProperties {
            get {
                return this.includedPropertiesField;
            }
            set {
                this.includedPropertiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("PropertieName", IsNullable=false)]
        public string[] ExcludedProperties {
            get {
                return this.excludedPropertiesField;
            }
            set {
                this.excludedPropertiesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ControlConfigurationsControlConfiguration {
        
        private string nameField;
        
        private string automationIdField;
        
        private string pathField;
        
        private string techPathField;
        
        private string controlTypeField;
        
        private ControlPropertiesControlPropertie[] interestingPropertiesField;
        
        private CompareScopes compareScopeField;
        
        private bool compareScopeFieldSpecified;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string AutomationId {
            get {
                return this.automationIdField;
            }
            set {
                this.automationIdField = value;
            }
        }
        
        /// <remarks/>
        public string Path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
        
        /// <remarks/>
        public string TechPath {
            get {
                return this.techPathField;
            }
            set {
                this.techPathField = value;
            }
        }
        
        /// <remarks/>
        public string ControlType {
            get {
                return this.controlTypeField;
            }
            set {
                this.controlTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ControlPropertie", IsNullable=false)]
        public ControlPropertiesControlPropertie[] InterestingProperties {
            get {
                return this.interestingPropertiesField;
            }
            set {
                this.interestingPropertiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CompareScopes CompareScope {
            get {
                return this.compareScopeField;
            }
            set {
                this.compareScopeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CompareScopeSpecified {
            get {
                return this.compareScopeFieldSpecified;
            }
            set {
                this.compareScopeFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class ControlPropertiesControlPropertie {
        
        private string nameField;
        
        private string valueField;
        
        private bool isValueRegExpField;
        
        private CompareScopes compareScopeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsValueRegExp {
            get {
                return this.isValueRegExpField;
            }
            set {
                this.isValueRegExpField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CompareScopes CompareScope {
            get {
                return this.compareScopeField;
            }
            set {
                this.compareScopeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    public enum CompareScopes {
        
        /// <remarks/>
        OnlyChildren,
        
        /// <remarks/>
        OnlyControl,
        
        /// <remarks/>
        ChildrenWithControl,
    }
}
