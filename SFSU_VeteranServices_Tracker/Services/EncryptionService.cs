/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: EncryptionService.cs
 * 
 * Description: I created an EncryptionService to separate encryption from the
 * rest of the application. It has three main responsibilities. First, 
 * GetEncryptionKeyAsync retrieves the application's encryption key from MAUI 
 * SecureStorage. If one doesn't exist yet, the application generates a random 
 * 256-bit key and stores it securely. Second, EncryptAsync converts normal 
 * text into bytes and encrypts those bytes using AES-GCM. Each encryption also
 * generates a random nonce and an authentication tag. Finally, DecryptAsync 
 * reverses the process using the same key, nonce, and tag to recover the original 
 * information.
 * 
 * ***************************************************************************/

using SFSU_VeteranServices_Tracker.Model;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Services
{

    // GetEncryptionKeyAsync()
    //                ↓
    // Gets or creates the secret key


    // EncryptAsync()
    //        ↓
    // Text → Bytes → AES-GCM → Encrypted text


    // DecryptAsync()
    //        ↓
    // Encrypted text → AES-GCM → Bytes → Original text

    public class EncryptionService
    {
        // label for the encryption key in secure storage
        private const string keyName = "VeteranServicesEncryptionKey";
        private async Task<byte[]> GetEncryptionKeyAsync()
        {
            // Check SecureStorage and see if we already created an encryption key
            string? savedKey = await SecureStorage.GetAsync(keyName);

            // no key found, generate a new one and save it
            if (string.IsNullOrEmpty(savedKey))
            {
                // this creates the key using a secure random number generator
                byte[] newKey = RandomNumberGenerator.GetBytes(32); // 256-bit key

                // convert the key to a base64 string for storage
                // SecureStorage is easier to work with as a string
                savedKey = Convert.ToBase64String(newKey);

                // save the new key to secure storage
                await SecureStorage.Default.SetAsync(keyName, savedKey);
            }

            return Convert.FromBase64String(savedKey);
        }
        public async Task<string> EncryptAsync(string text)
        {
            // Get the encryption key from secure storage
            byte[] key = await GetEncryptionKeyAsync();

            // Advanced Encryption Standard (AES) is a symmetric encryption algorithm that
            // uses the same key for both encryption and decryption.
            // Galois/Counter Mode (GCM) is a mode of operation for AES that provides both
            // authentication and encryption.


            // AES doesn't understand strings, so we need to convert the string to bytes
            byte[] plainTextBytes =
                Encoding.UTF8.GetBytes(text);

            // nonce is a random unique value ussed every time we encrypt something.
            // It ensures that the same plaintext will produce different ciphertexts each time.
            byte[] nonce =
                RandomNumberGenerator.GetBytes(12);

            // AES will put the encrypted data in this array
            byte[] encryptedBytes =
                new byte[plainTextBytes.Length];

            // AES-GCM produces an authentication tag that ensures the integrity and
            // authenticity of the data.
            byte[] tag =
                new byte[16];

            // Create an AES-GCM object using the encryption key.
            // 16 represents the size of the authentication tag in bytes.
            using AesGcm aes =
                new AesGcm(key, 16);

            // Encrypt the plaintext bytes and store the result in the encryptedBytes array
            aes.Encrypt(
                nonce,
                plainTextBytes,
                encryptedBytes,
                tag);

            // Encryption Key
            //        +
            // Nonce
            //        +
            // "Clark Batungbakal"
            //        ↓
            // AES - GCM
            //        ↓
            // Encrypted Bytes
            //        +
            // Authentication Tag

            return
                Convert.ToBase64String(nonce) + "|" +
                Convert.ToBase64String(tag) + "|" +
                Convert.ToBase64String(encryptedBytes);
        }
        public async Task<string> DecryptAsync(string encryptedText)
        {
            // Get the encryption key from secure storage
            byte[] key =  await GetEncryptionKeyAsync();

            // Split the encrypted text into its components: nonce, tag, and encrypted bytes
            string[] parts =
                encryptedText.Split('|');

            // Validate that the encrypted text is in the correct format
            if (parts.Length != 3)
            {
                throw new Exception(
                    "Encrypted data is not in the correct format.");
            }

            // Convert the base64-encoded components back to byte arrays
            byte[] nonce =
                Convert.FromBase64String(parts[0]);
            
            // Authentication Tag
            byte[] tag =
                Convert.FromBase64String(parts[1]);

            // Encrypted Bytes
            byte[] encryptedBytes =
                Convert.FromBase64String(parts[2]);

            // Create an array to hold the decrypted bytes
            byte[] decryptedBytes =
                new byte[encryptedBytes.Length];

            // Create an instance of the AesGcm class with the encryption key and nonce
            using AesGcm aes =
                new AesGcm(key, 16);

            // Decrypt the encrypted bytes and store the result in the decryptedBytes array
            aes.Decrypt(
                nonce,
                encryptedBytes,
                tag,
                decryptedBytes);

            // Encrypted Data
            //        +
            // Nonce
            //        +
            // Tag
            //        +
            // Secret Key
            //        ↓
            // AES - GCM
            //        ↓
            // Original Bytes

            return Encoding.UTF8.GetString(
                decryptedBytes);
        }

    }
}
