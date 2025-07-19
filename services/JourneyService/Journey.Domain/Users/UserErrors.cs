using Journey.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journey.Domain.Users
{
    public static class UserErrors
    {
        public static readonly Error EmailNotUnique = Error.Conflict(
            "Users.EmailNotUnique", "This email already exists. Try another one!");

        public static readonly Error FailedCreating = Error.Failure(
            "Users.FailedCreating", "Failed creating user. Try again later!");

        public static readonly Error FailedSorting = Error.Failure(
            "Users.FailedSorting", "Failed on sorting users.");

        public static readonly Error NotLoggedIn = Error.Failure(
            "Users.NotLoggedIn", "User is not logged in. Please log in to continue!");
        public static readonly Error NotFound = Error.NotFound(
            "Users.NotFound", "There is no user with the specified identifier");

        public static readonly Error EmailConfirmed = Error.Failure(
            "Users.EmailConfirmed","This user's email is already confirmed");

        public static readonly Error LoginFailed = Error.Failure(
            "Users.LoginFailure", "Email or password is incorrect!");

        public static readonly Error UserManagerFail = Error.Failure(
            "Users.UserManagerFail", "UserManager is null!");

        public static readonly Error AccountLocked = Error.Failure(
            "Users.AccountLocked", "Your account is locked. Please reset your password!");

        public static readonly Error ResetPasswordFailed = Error.Failure(
            "Users.ResetPasswordFailed", "Reset password failed. The token provided is invalid");

        public static readonly Error ResetPassword = Error.Failure(
            "Users.ResetPassword", "Reset password failed. Can not reset password for non active users!");

        public static readonly Error ForgetPassword = Error.Failure(
            "Users.ForgetPassword", "Forget password failed. Can not forget password for non active users!");

        public static readonly Error ResetPasswordFields = Error.Failure(
            "Users.ResetPasswordFields", "Reset password failed. Fields are empty!");

        public static readonly Error NewConfirmEmail = Error.Failure(
            "Users.NewConfirmEmail", "New email confirmation is sent!!");

        public static readonly Error RoleUpdateFailed = Error.Failure(
            "Users.RoleUpdateFailed", "Role update failed.");

        public static readonly Error EmailUpdateFailed = Error.Failure(
            "Users.EmailUpdateFailed", "Email update failed.");

        public static readonly Error IdToActivateEmpty = Error.Failure(
            "Users.IdToActivateEmpty", "Id should not be empty!");

        public static readonly Error NoAuthPassChange = Error.Forbidden(
            "Users.NoAuthorization","You don't have authorization to change password of another user!");
     
    }

}
