using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace ShowHash
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text += " Ver.1.1.0";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var item in new string[] { "ファイル名", "サイズ", "作成日時",  "更新日時", "ハッシュ値（SHA-1）" })
            { 
                var column = new DataGridViewTextBoxColumn();
                column.Name = item;
                //column.HeaderText = item;
                dataGridView1.Columns.Add(column);
            }
            dataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Gainsboro;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Font = new Font("Consolas", 10);

            // アイコンにファイルがドロップされた場合の処理
            var args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                //MessageBox.Show($"Got {String.Join(",", files)}");
                SetFileInfo(args.Skip(1));
            }
            ActiveControl = dataGridView1;
        }

        private void SetFileInfo(IEnumerable<string> files)
        {
            var folderFiles = files.Where(x => Directory.Exists(x))
                                              .SelectMany(x => Directory.GetFiles(x, "*", SearchOption.AllDirectories));
            var planeFiles = files.Where(x => !Directory.Exists(x));

            foreach (var file in folderFiles.Concat(planeFiles))
            {
                if (Directory.Exists(file)) continue; // フォルダの場合はいったんスキップ
                var str = System.IO.File.ReadAllText(file);
                byte[] beforeByteArray = Encoding.UTF8.GetBytes(str);

                // SHA1 ハッシュ値を計算する
                SHA1 sha1 = SHA1.Create();
                byte[] afterByteArray1 = sha1.ComputeHash(beforeByteArray);
                sha1.Clear();
                StringBuilder sb1 = new StringBuilder();
                foreach (byte b in afterByteArray1)
                {
                    sb1.Append(b.ToString("X2"));
                }
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                dataGridView1.Rows.Add($"{fi.Name}", $"{fi.Length:#,0}", $"{fi.CreationTime}", $"{fi.LastAccessTime}", $"{sb1}");
            }
            dataGridView1.ClearSelection();
        }

        //
        // 「終了」ボタン
        //
        private void button2_Click(object sender, EventArgs e)
            {
                this.Close();
            }
        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2.PerformClick();
        }

        //
        // 「クリア」ボタン
        //
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void クリアToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
        }


        private void 開くToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "ファイルを開く";
            op.InitialDirectory = @"C:\";
            op.Multiselect = true;
            op.Filter = "すべてのファイル(*.*)|*.*";
            var result = op.ShowDialog();

            if (result == DialogResult.OK)
            {
                string[] files = op.FileNames;
                SetFileInfo(files);
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            dataGridView1.ClearSelection();
        }


        //
        // Drag & Dropのインベントハンドラ
        //
        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            SetFileInfo(fileName);
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }


    }
}
