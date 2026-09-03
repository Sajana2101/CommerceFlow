using CommerceFlow.Domain.Enums;

namespace CommerceFlow.Domain.Entities
{
    public sealed class Customer
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private Customer()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
        }

        public Customer(
            string firstName,
            string lastName,
            string email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException(
                    "First name is required.",
                    nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException(
                    "Last name is required.",
                    nameof(lastName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email is required.",
                    nameof(email));

            Id = Guid.NewGuid();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = string.Empty;
            Role = UserRole.Customer;
            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(
                    "Password hash is required.",
                    nameof(passwordHash));

            PasswordHash = passwordHash;
        }
    }
}