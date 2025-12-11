using MoutsTI.Domain.Entities.Interfaces;
using System.Text.RegularExpressions;

namespace MoutsTI.Domain.Entities
{
    public class EmployeePhoneModel : IEmployeePhoneModel
    {
        public long PhoneId { get; private set; }
        public long EmployeeId { get; private set; }
        public string Phone { get; private set; }

        // Construtor privado para garantir que o objeto seja criado apenas através de métodos de fábrica
        private EmployeePhoneModel(long phoneId, long employeeId, string phone)
        {
            PhoneId = phoneId;
            EmployeeId = employeeId;
            Phone = phone;
        }

        // Construtor para criação de novo telefone (sem ID)
        private EmployeePhoneModel(long employeeId, string phone)
        {
            ValidatePhone(phone);

            EmployeeId = employeeId;
            Phone = NormalizePhone(phone);
        }

        // Factory method para criar um novo telefone
        public static EmployeePhoneModel Create(long employeeId, string phone)
        {
            return new EmployeePhoneModel(employeeId, phone);
        }

        // Factory method para reconstruir um telefone existente (da base de dados)
        public static EmployeePhoneModel Load(long phoneId, long employeeId, string phone)
        {
            ValidatePhone(phone);

            return new EmployeePhoneModel(phoneId, employeeId, phone);
        }

        // Método de negócio para atualizar o número de telefone
        public void UpdatePhone(string phone)
        {
            ValidatePhone(phone);
            Phone = NormalizePhone(phone);
        }

        private static void ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number cannot be empty.", nameof(phone));

            var normalizedPhone = NormalizePhone(phone);

            if (normalizedPhone.Length > 25)
                throw new ArgumentException("Phone number cannot exceed 25 characters.", nameof(phone));

            if (normalizedPhone.Length < 8)
                throw new ArgumentException("Phone number must have at least 8 digits.", nameof(phone));

            // Valida se contém apenas números, espaços, parênteses, hífens e sinal de mais
            if (!Regex.IsMatch(phone, @"^[\d\s\(\)\-\+]+$"))
                throw new ArgumentException("Phone number contains invalid characters.", nameof(phone));
        }

        // Normaliza o telefone removendo caracteres especiais, mas mantendo espaços e formatação básica
        private static string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            // Remove espaços extras no início e fim
            return phone.Trim();
        }

        // Método utilitário para obter apenas os dígitos do telefone
        public string GetDigitsOnly()
        {
            return Regex.Replace(Phone, @"[^\d]", string.Empty);
        }

        // Método para validar se o telefone é um celular (regra brasileira - 11 dígitos)
        public bool IsMobilePhone()
        {
            var digits = GetDigitsOnly();
            return digits.Length == 11 && (digits[2] == '9');
        }

        // Método para validar se o telefone é fixo (regra brasileira - 10 dígitos)
        public bool IsLandline()
        {
            var digits = GetDigitsOnly();
            return digits.Length == 10;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not EmployeePhoneModel other)
                return false;

            return PhoneId == other.PhoneId;
        }

        public override int GetHashCode()
        {
            return PhoneId.GetHashCode();
        }

        public override string ToString()
        {
            return Phone;
        }
    }
}
