using System.Collections.Generic;

namespace SmallApi.Domain.Infra
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Result()
        {
            this.Success = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<string> _businessErrorMessages = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasException { get { return !string.IsNullOrEmpty(this.ExceptionMessage); } }

        /// <summary>
        /// 
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasBusinessErrors
        {
            get { return this._businessErrorMessages.Count > 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> BusinessErrorMessages
        {
            get { return _businessErrorMessages; }
            set { _businessErrorMessages = value; }
        }
    }
}
