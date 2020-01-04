using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsVinhos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await this.getAllVinhos();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private string URI;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.URI = "http://localhost:26313/api/Vinhos";
        }

        //pacote : microsoft.AspNet.WebApi.Client
        private async Task getAllVinhos()
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(this.URI))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var VinhosJsonString = await response.Content.ReadAsStringAsync();

                        this.dataGridView1.DataSource = JsonConvert.DeserializeObject<Vinhos[]>(VinhosJsonString).ToList();
                        
                    }else
                    {
                        MessageBox.Show("Falha na comunicação:" + response.StatusCode);
                    }
                }
            }

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await this.addVinho();
        }

        private async Task addVinho()
        {
            Vinhos vinho = new Vinhos();
            vinho.Nome_vinho = this.txtNome.Text;
            vinho.Idade_vinho = Convert.ToInt32(this.txtIdade.Text);
            vinho.Preco_vinho = Convert.ToDecimal(this.txtPreco.Text);

            using ( var client = new HttpClient())
            {
                var VinhoSerialized = JsonConvert.SerializeObject(vinho);

                var content = new StringContent(VinhoSerialized, Encoding.UTF8, "Application/json");
                var result = await client.PostAsync(this.URI, content);


            }
            await this.getAllVinhos();

        }

        private async Task UpdateVinho()
        {
            Vinhos vinho = new Vinhos();
            vinho.Cod_vinho = Convert.ToInt32(this.txtCodigo.Text);
            vinho.Nome_vinho = this.txtNome.Text;
            vinho.Idade_vinho = Convert.ToInt32(this.txtIdade.Text);
            vinho.Preco_vinho = Convert.ToDecimal(this.txtPreco.Text);

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(this.URI + '/' + vinho.Cod_vinho, vinho);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("vinho atualizado");
                    await getAllVinhos();
                }
                else
                {
                    MessageBox.Show("Falha na atualização" + response.StatusCode);

                }
            }

        }

        private async Task RemoveVinho()
        {
            int cod_vinho = Convert.ToInt32(this.txtCodigo.Text);
            using ( var client = new HttpClient()){
                client.BaseAddress = new Uri(this.URI);
                HttpResponseMessage response = await client.DeleteAsync( String.Format("{0}/{1}", client.BaseAddress, cod_vinho));
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Vinho Removido");
                    await getAllVinhos();
                }else
                {
                    MessageBox.Show("Falha na atualização " + response.StatusCode);
                }
            }

        }

        private async Task GetVinhoById(int  cod_vinho)
        {
            using ( var client = new HttpClient()){
                BindingSource bsDados = new BindingSource();
                String endereco = this.URI + "/" + cod_vinho.ToString();
                HttpResponseMessage response = await client.GetAsync(endereco);

                if (response.IsSuccessStatusCode)
                {
                    var VinhoJson = await response.Content.ReadAsStringAsync();

                    bsDados.DataSource = JsonConvert.DeserializeObject<Vinhos>(VinhoJson);
                    this.dataGridView1.DataSource = bsDados;
                }
                else
                {
                    MessageBox.Show("Falha na Buscar " + response.StatusCode);
                }
            }

        }


        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            txtCodigo.Text = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtNome.Text = this.dataGridView1.CurrentRow.Cells[1].Value.ToString();
            txtIdade.Text = this.dataGridView1.CurrentRow.Cells[2].Value.ToString();
            txtPreco.Text = this.dataGridView1.CurrentRow.Cells[3].Value.ToString();


        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.UpdateVinho();
            this.getAllVinhos();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await this.RemoveVinho();
            await this.getAllVinhos();

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await this.GetVinhoById( int.Parse(this.txtCodigo.Text));
        }
    }
}
