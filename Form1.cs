using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FileEncrypter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtInput.Text = openFileDialog1.FileName;
                txtOutput.Text = txtInput.Text + ".bin";
            }
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutput.Text = saveFileDialog1.FileName;
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            Crypto tmp = new Crypto();
            if (txtKey.Text.Length <= 0)
            {
                if (MessageBox.Show("Seems you didn't create a Pass Phrase, would you like on generated for you?", "Blank Pass Phrase Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    //Generates a key that may contain none standard characters, and one unlikely to be remembered
                    //txtKey.Text = tmp.GenerateKey();
                    
                    //not a great key, but it will do, much more readable
                    txtKey.Text = GetRandomString(16);
                }
                else
                {
                    txtKey.Select();
                    return;
                }
            }

             tmp.Encrypting(txtInput.Text, txtOutput.Text, txtKey.Text);
             MessageBox.Show("Remember your Pass Phrase, or you won't be able to decrypt the file");
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            // Decrypt the file.

            if (txtKey.Text.Length <= 0)
            {
                MessageBox.Show("Pass Phrase Can Not be blank during decryption");
            }
            else
            {
                Crypto tmp = new Crypto();
                tmp.Decrypting(txtInput.Text, txtOutput.Text, txtKey.Text);
            }
            
        }

        private void txtOutput_TextChanged(object sender, EventArgs e)
        {
            validationCheck();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            validationCheck();
        }

        private void validationCheck()
        {
            if (txtOutput.Text.Length > 0 && txtInput.Text.Length > 0)
            {
                
                
                btnDecrypt.Enabled = true;
                btnEncrypt.Enabled = true;                
            }
            else
            {
                btnDecrypt.Enabled = false;
                btnEncrypt.Enabled = false;
            }

        }

        public string GetRandomString(int length)
        {
            Random rnd = new Random();

                /*set default values to upper/lower alphanumeric and special*/
                string chars = "!@#$%^*=-+?.0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";


            StringBuilder rs = new StringBuilder();
            while (length > 0)
            {
                rs.Append(chars[(int)(rnd.NextDouble() * chars.Length)]);
                length--;
            }
            return rs.ToString();
        }


    }
}
