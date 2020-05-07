﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOVA2020.objs.dbitems;
using System.Drawing.Printing;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using System.Drawing.Imaging;

namespace MOVA2020.forms
{
    public partial class Laskutus : Form
    {
        private Lasku l;
        private Primary p;
        Bitmap bitmap;
        //Laskutuksen koodi.


        public Laskutus(Primary p, Lasku l)
        {
            InitializeComponent();
            this.p = p;
            this.l = l;
            if(this.l.Varaus.Vahvistus_pvm.Equals(DateTime.Parse("1970-01-01 00:00:00")))
            {
                btnVarmenna.Enabled = true;
            }
            TByht.Text = l.Summa.ToString();
            TBAsiakas.Text = l.Varaus.Asiakas.ToString();
            TBnum.Text = l.Lasku_id.ToString();
            TBerapvm.Text = l.Erapaiva.ToString("dd-MM-yyyy");
            TBpvm.Text = l.Varaus.Vahvistus_pvm.ToString("dd-MM-yyyy");
            string lisatiedot = l.Varaus.Mokki.Kuvaus + "\r\n" + l.Varaus.Mokki.Varustelu + "\r\n";
            string summat = l.Varaus.Alkupvm_varaus.ToString("yyyy-MM-dd")+" - "+l.Varaus.Loppupvm_varaus.ToString("yyyy-MM-dd")+ "\r\n";
            summat += (l.Varaus.Loppupvm_varaus - l.Varaus.Alkupvm_varaus).TotalDays.ToString() + " päivä(ä), " +
                l.Varaus.Mokki.Hinta * (l.Varaus.Loppupvm_varaus - l.Varaus.Alkupvm_varaus).TotalDays;


            foreach (KeyValuePair<int, int> item in l.Varaus.Varauksenpalvelut)
            {
                Palvelu pa = this.p.Palvelut.Find(i => i.Palvelu_id == item.Key);
                lisatiedot += "\r\n" + pa.Nimi + pa.Hinta.ToString() + " €/kpl\r\nx" + item.Value;
                summat += "\r\nx" + item.Value +" * " + pa.Hinta.ToString() + " = "+(item.Value * pa.Hinta);
            }
            TBlisatiedot.Text = lisatiedot;
            summat += "\r\nSumma (ilman ALV):" + l.SummaEiAlv;
            summat += "\r\nALV (24%):" + l.Alv;
            TBLaskutus.Text = summat;
        }
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(bitmap, e.MarginBounds);
        }
        private void bttulosta_Click(object sender, EventArgs e)
        {
            Graphics grp = CreateGraphics();
            Size formSize = this.panelLasku.Size;
            bitmap = new Bitmap(formSize.Width, formSize.Height);
            panelLasku.DrawToBitmap(bitmap, new Rectangle(0, 0, formSize.Width, formSize.Height));
            grp = Graphics.FromImage(bitmap);
      
            printpreview.Document = printDocument;
            printpreview.ClientSize = bitmap.Size;
            printpreview.PrintPreviewControl.Zoom = 1;
            printpreview.ShowDialog();
        }
        private void btlaheta_Click(object sender, EventArgs e)
        {
            //Nappi, joka aukaisee lomakkeen, jolla kysytään lähetyssähköpostia
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\MOVA2020";
            System.IO.Directory.CreateDirectory(path);
            string filename = (DateTime.Now).ToString("yyyy-MM-dd") + "_" + this.l.Varaus.Varaus_id.ToString() + ".png";
            bitmap.Save(path+"\\"+filename, ImageFormat.Png);

            sähkoposti sp = new sähkoposti(path+"\\"+filename);
            sp.Show();
        }

        private void btnVaraustiedot_Click(object sender, EventArgs e)
        {
            Varauksentiedot vt = new Varauksentiedot(p, l.Varaus);
            vt.Show();
        }

        private void btnVarmenna_Click(object sender, EventArgs e)
        {
            Varaus var = this.l.Varaus;
            DialogResult dr = MessageBox.Show("Onko kyseinen maksu suoritettu ?", "Maksun varmennus", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                Dictionary<string, object> pairs = new Dictionary<string, object>();
                pairs.Add("$vahvistus_pvm", DateTime.Now);
                pairs.Add("$varaus_id", var.Varaus_id);
                string query = "UPDATE varaus SET vahvistus_pvm=$vahvistus_pvm WHERE varaus_id=$varaus_id";
                this.p.Db.DMquery(query, pairs);
                MessageBox.Show("Maksu on merkattu suoritetuksi!", "Maksun varmennus", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnVarmenna.Enabled = false;
                this.p.paivita();
            }
        }

        private void TBviitenum_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
