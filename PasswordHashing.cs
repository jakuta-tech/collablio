using System;
using System.Security.Cryptography;


namespace collablio
{
	public class PasswordHashing
	{
	  private const int SaltSize = 16; // 128 bit 
	  private const int KeySize = 32; // 256 bit  <-- shouldnt this be 512bit for SHA512??
	  private const int Iterations = 10000;
	  
	  public PasswordHashing()
	  {
	  }

	  public static string Hash(string password)
	  {
		using (var algorithm = new Rfc2898DeriveBytes(
		  password,
		  SaltSize,
		  Iterations,
		  HashAlgorithmName.SHA512))
		{
		  var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
		  var salt = Convert.ToBase64String(algorithm.Salt);

		  return $"{Iterations}.{salt}.{key}";
		}
	  }

	  public static bool Check(string hash, string password)
	  {
		var parts = hash.Split('.', 3);

		if (parts.Length != 3)
		{
		  throw new FormatException("Unexpected hash format. " + 
			"Should be formatted as `{iterations}.{salt}.{hash}`");
		}

		var iterations = Convert.ToInt32(parts[0]);
		var salt = Convert.FromBase64String(parts[1]);
		var key = Convert.FromBase64String(parts[2]);

		using (var algorithm = new Rfc2898DeriveBytes(
		  password,
		  salt,
		  iterations,
		  HashAlgorithmName.SHA512))
		{
		  var keyToCheck = algorithm.GetBytes(KeySize);

		  var verified = Convert.ToBase64String(keyToCheck) == parts[2];

		  return verified;
		}
	  }
	}
}