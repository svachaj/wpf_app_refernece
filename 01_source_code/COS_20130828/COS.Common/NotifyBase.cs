using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq.Expressions;

namespace COS.Common
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

    interface ITabletViewModel
    {
        Action<object> OKAction { set; get; }

        Action CancelAction { set; get; }

        string Title { set; get; }

        bool IsModal { set; get; }

    }

    public class TabletViewModelBase : NotifyBase, ITabletViewModel
    {
        public TabletViewModelBase()
            : base()
        {

        }


        public Action<object> OKAction
        {
            set;
            get;
        }

        public Action CancelAction
        {
            set;
            get;
        }

        public object Data { set; get; }

        private string _title = "";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        private bool _isModal = false;
        public bool IsModal
        {
            get
            {
                return _isModal;
            }
            set
            {
                if (_isModal != value)
                {
                    _isModal = value;
                    OnPropertyChanged("IsModal");
                }
            }
        }
    }



    public class ValidationViewModelBase : NotifyBase,
       IDataErrorInfo
    {
        private readonly Dictionary<string,
        Func<ValidationViewModelBase, object>> propertyGetters;
        private readonly Dictionary<string, ValidationAttribute[]> validators;

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        public string this[string propertyName]
        {
            get
            {
                if (this.propertyGetters.ContainsKey(propertyName))
                {
                    var propertyValue = this.propertyGetters[propertyName](this);
                    var errorMessages = this.validators[propertyName]
                        .Where(v => !v.IsValid(propertyValue))
                        .Select(v => v.ErrorMessage).ToArray();

                    return string.Join(Environment.NewLine, errorMessages);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get
            {
                var errors = from validator in this.validators
                             from attribute in validator.Value
                             where !attribute.IsValid(this.propertyGetters
                [validator.Key](this))
                             select attribute.ErrorMessage;

                return string.Join(Environment.NewLine, errors.ToArray());
            }
        }

        /// <summary>
        /// Gets the number of properties which have a 
        /// validation attribute and are currently valid
        /// </summary>
        public int ValidPropertiesCount
        {
            get
            {
                var query = from validator in this.validators
                            where validator.Value.All(attribute =>
              attribute.IsValid(this.propertyGetters[validator.Key](this)))
                            select validator;

                var count = query.Count() - this.validationExceptionCount;
                return count;
            }
        }

        /// <summary>
        /// Gets the number of properties which have a validation attribute
        /// </summary>
        public int TotalPropertiesWithValidationCount
        {
            get
            {
                return this.validators.Count();
            }
        }

        public ValidationViewModelBase()
        {
            this.validators = this.GetType()
                .GetProperties()
                .Where(p => this.GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, p => this.GetValidations(p));

            this.propertyGetters = this.GetType()
                .GetProperties()
                .Where(p => this.GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, p => this.GetValueGetter(p));
        }

        private ValidationAttribute[] GetValidations(PropertyInfo property)
        {
            return (ValidationAttribute[])property.GetCustomAttributes
            (typeof(ValidationAttribute), true);
        }

        private Func<ValidationViewModelBase, object>
            GetValueGetter(PropertyInfo property)
        {
            return new Func<ValidationViewModelBase, object>
            (viewmodel => property.GetValue(viewmodel, null));
        }

        private int validationExceptionCount;

        public void ValidationExceptionsChanged(int count)
        {
            this.validationExceptionCount = count;
            this.OnPropertyChanged("ValidPropertiesCount");
        }


        private EditMode _editingMode = EditMode.View;
        /// <summary>
        /// Gets or sets the edititng mode.
        /// </summary>
        /// <value>The edititng mode.</value>
        public EditMode EditingMode
        {
            set
            {
                if (_editingMode != value)
                {
                    _editingMode = value;
                    OnPropertyChanged("EditingMode");
                }
            }
            get
            {
                return _editingMode;
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
            get
            {
                return _isBusy;
            }
        }

        private string _raiseErrors = "";
        public string RaiseErrors
        {
            set
            {
                _raiseErrors = value;
                OnPropertyChanged("RaiseErrors");
            }
            get
            {
                return _raiseErrors;
            }
        }

        private string _raiseConfirm = "";
        public string RaiseConfirm
        {
            set
            {
                _raiseConfirm = value;
                OnPropertyChanged("RaiseConfirm");
            }
            get
            {
                return _raiseConfirm;
            }
        }

    }

    public enum EditMode
    {
        View = 1,
        New = 2,
        Edit = 3,
        AllMode = 4
    }
}
