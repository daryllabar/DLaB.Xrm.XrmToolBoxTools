using System;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Contains the Validator and the Exception to throw
    /// </summary>
    public struct AssertValidator
    {
        /// <summary>
        /// The Validator
        /// </summary>
        public IRequirementValidator Validator { get; set; }
        /// <summary>
        /// The Exception
        /// </summary>
        public Exception ExceptionToThrow { get; set; }
        /// <summary>
        /// The Exception Factory
        /// </summary>
        public Func<InvalidRequirementReason, IExtendedPluginContext, Exception> ExceptionFactory { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validator">The Validator</param>
        /// <param name="exceptionToThrow">The Exception to Throw</param>
        public AssertValidator(IRequirementValidator validator, Exception exceptionToThrow)
        {
            ExceptionFactory = null;
            ExceptionToThrow = exceptionToThrow ?? throw new ArgumentNullException(nameof(exceptionToThrow)); ;
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validator">The Validator</param>
        /// <param name="errorMessage">The error message to Throw as an InvalidPluginExecutionException</param>
        public AssertValidator(IRequirementValidator validator, string errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }
            ExceptionFactory = null; 
            ExceptionToThrow = new InvalidPluginExecutionException(errorMessage);
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <param name="errorFactory">Function that creates an exception based on the InvalidColumnRequirementReason</param>
        public AssertValidator(IRequirementValidator validator, Func<InvalidRequirementReason, IExtendedPluginContext, Exception> errorFactory)
        {
            ExceptionFactory = errorFactory ?? throw new ArgumentException(nameof(errorFactory));
            ExceptionToThrow = null;
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }
    }
}
