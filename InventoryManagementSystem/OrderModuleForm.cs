﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class OrderModuleForm : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zafer\Documents\dbIMS.mdf;Integrated Security=True;Connect Timeout=30");
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        double qty = 0;
        public OrderModuleForm()
        {
            InitializeComponent();
            LoadCustomer();
            LoadProduct();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();

        }
        public void LoadCustomer()
        {
            int i = 0;
            dgvCustomer.Rows.Clear();
            cm = new SqlCommand("SELECT cid, cname FROM tbcustomer WHERE CONCAT(cid, cname) LIKE '%" + txtSearchCust.Text + "%'", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvCustomer.Rows.Add(i, dr[0].ToString(), dr[1].ToString());
            }
            dr.Close();
            con.Close();
        }
        public void LoadProduct()
        {
            int i = 0;
            dgvProduct.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tbProduct WHERE CONCAT(pid, pname, pprice, pdescription, pcategory) LIKE '%" + txtSearchProd.Text + "%'", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void txtSearchCust_TextChanged(object sender, EventArgs e)
        {
            LoadCustomer();
        }

        private void txtSearchProd_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GetQty();
            if (Convert.ToDouble(UDQty.Value) > qty)
            {
                MessageBox.Show("In stock quantity is not enough!!", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UDQty.Value = UDQty.Value - 1;
                return;
            }
            if ((UDQty.Value) > 0)
            {
                double total = Convert.ToDouble(txtPrice.Text) * Convert.ToDouble(UDQty.Value);
                txtTotal.Text = total.ToString();
            }

        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtCId.Text = dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtCName.Text = dgvCustomer.Rows[e.RowIndex].Cells[2].Value.ToString();
        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtpid.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtPname.Text = dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtPrice.Text = dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString();
            
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCId.Text == "")
                {
                    MessageBox.Show("Please select customer !! ", "warnig", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }
                if (txtpid.Text == "")
                {
                    MessageBox.Show("Please select Product !! ", "warnig", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }
                if (MessageBox.Show("Are you sure you want to insert this order?", "Saving Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cm = new SqlCommand("INSERT INTO tbtOrder(odate ,pid ,cid ,qty, price,total)VALUES(@odate ,@pid ,@cid ,@qty,@price,@total)", con);
                    cm.Parameters.AddWithValue("@odate", dtOrder.Value);
                    cm.Parameters.AddWithValue("@pid", Convert.ToInt32(txtpid.Text));
                    cm.Parameters.AddWithValue("@cid", Convert.ToInt32(txtCId.Text));
                    cm.Parameters.AddWithValue("@qty", Convert.ToDouble(UDQty.Value));
                    cm.Parameters.AddWithValue("@price", Convert.ToDouble(txtPrice.Text));
                    cm.Parameters.AddWithValue("@total", Convert.ToDouble(txtTotal.Text));
                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("order has been successfully inserted");
                    
                    cm = new SqlCommand("UPDATE tbProduct SET  pqty=(pqty-@pqty)  WHERE pid LIKE '" + txtpid.Text + "' ", con);                   
                    cm.Parameters.AddWithValue("@pqty", Convert.ToDouble(UDQty.Text));                 
                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();
                    Clear();
                    LoadProduct();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Clear()
        {
            txtCId.Clear();
            txtCName.Clear();

            txtpid.Clear();
            txtPname.Clear();

            txtPrice.Clear();
            UDQty.Value = 0;
            txtTotal.Clear();
            dtOrder.Value = DateTime.Now;

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();          
        }
        public void GetQty()
        {
            cm = new SqlCommand("SELECT * FROM tbProduct WHERE pid='" + txtpid.Text + "'", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                qty = Convert.ToDouble(dr[2].ToString()); 
            }
            dr.Close();
            con.Close();
        }

       
    }
}
