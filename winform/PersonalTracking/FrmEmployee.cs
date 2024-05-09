using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using DAL;
using DAL.DTO;
using System.IO;

namespace PersonalTracking
{
    public partial class FrmEmployee : Form
    {
        public FrmEmployee()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textUserNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void txtSalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        EmployeeDTO dto =new EmployeeDTO();
        public EmployeeDetailDTO detail = new EmployeeDetailDTO();
        public bool isUpdate = false;
        string imagepath = "";
        private void FrmEmployee_Load(object sender, EventArgs e)
        {
             dto = EmployeeBLL.GetAll();
            cmbDepartment.DataSource = dto.Departments;
            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "ID";
            cmbPosition.DataSource = dto.Positions;
            cmbPosition.DisplayMember = "PositionName";
            cmbPosition.ValueMember = "ID";
            cmbPosition.SelectedIndex = -1;
            cmbDepartment.SelectedIndex = -1;
            combofull = true;
            if(isUpdate)
            {
                txtName.Text = detail.Name;
                txtSurname.Text = detail.Surname;
                textUserNo.Text = detail.UserNo.ToString();
                txtPassword.Text = detail.Password;
                chAdmin.Checked= Convert.ToBoolean(detail.isAdmin);
                txtAddress.Text = detail.Address;
                dateTimePicker1.Value = Convert.ToDateTime(detail.Birthday);
                cmbDepartment.SelectedValue= detail.DepartmentID;
                cmbPosition.SelectedValue= detail.PositionID;
                txtSalary.Text = detail.Salary.ToString();
                imagepath = Application.StartupPath+"\\images\\"+detail.ImagePath;
                txtImagePath.Text = imagepath;
                pictureBox1.ImageLocation=imagepath;
                if(!UserStatic.isAdmin)
                {
                    chAdmin.Enabled = false;
                    textUserNo.Enabled = false;
                    txtSalary.Enabled = false;
                    cmbDepartment.Enabled = false;
                    cmbPosition.Enabled = false;
                }
            }
        }

        bool combofull = false;
        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combofull)
            {
                int departmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                cmbPosition.DataSource = dto.Positions.Where(x => x.DepartmentID == departmentID).ToList();
            }
        }

        string fileName = "";
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK) 
            {
                pictureBox1.Load(openFileDialog1.FileName);
                txtImagePath.Text  = openFileDialog1.FileName;
                string Unique=Guid.NewGuid().ToString();
                fileName += Unique + openFileDialog1.SafeFileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (textUserNo.Text.Trim() == "")
                MessageBox.Show("User no is empty");            
            
            else if (txtPassword.Text.Trim() == "")
                MessageBox.Show("Password is empty");
            else if (txtName.Text.Trim() == "")
                MessageBox.Show("Name is empty");
            else if (txtSurname.Text.Trim() == "")
                MessageBox.Show("Surname is empty");
            else if (txtSalary.Text.Trim() == "")
                MessageBox.Show("Salary is empty");
            else if (cmbDepartment.SelectedIndex == -1)
                MessageBox.Show("Selected a department");
            else if (cmbPosition.SelectedIndex == -1)
                MessageBox.Show("Selected a position");
            else
            {
                if(!isUpdate)
                {
                   if (!EmployeeBLL.isUnique(Convert.ToInt32(textUserNo.Text)))
                        MessageBox.Show("This user no is used by another employee, please change");
                    else
                    {
                        EMPLOYEE employee = new EMPLOYEE();
                        employee.UserNo = Convert.ToInt32(textUserNo.Text);
                        employee.Password = txtPassword.Text;
                        employee.isAdmin = chAdmin.Checked;
                        employee.Name = txtName.Text;
                        employee.SurName = txtSurname.Text;
                        employee.Salary = Convert.ToInt32(txtSalary.Text);
                        employee.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                        employee.PositionID = Convert.ToInt32(cmbPosition.SelectedValue);
                        employee.Address = txtAddress.Text;
                        employee.Birthday = dateTimePicker1.Value;
                        employee.ImagePath = fileName;
                        EmployeeBLL.AddEmployee(employee);
                        File.Copy(txtImagePath.Text, @"images\\" + fileName);
                        MessageBox.Show("Employee was added");
                        textUserNo.Clear();
                        txtPassword.Clear();
                        chAdmin.Checked = false;
                        txtName.Clear();
                        txtSurname.Clear();
                        txtSalary.Clear();
                        txtAddress.Clear();
                        txtImagePath.Clear();
                        pictureBox1.Image = null;
                        combofull = false;
                        cmbDepartment.SelectedIndex = -1;
                        cmbPosition.SelectedIndex = -1;
                        cmbPosition.DataSource = dto.Positions;
                        combofull = true;
                        dateTimePicker1.Value = DateTime.Today;
                    }
                    
                }
                else
                {
                    DialogResult result = MessageBox.Show("are you sure?", "warning", MessageBoxButtons.YesNo);
                    if(result==DialogResult.Yes)
                    {
                        EMPLOYEE employee = new EMPLOYEE();
                        if(txtImagePath.Text!=imagepath)
                        {
                            if(File.Exists(@"images\\" + detail.ImagePath))
                               File.Delete(@"images\\" + detail.ImagePath);


                            File.Copy(txtImagePath.Text, @"images\\" + fileName);
                            employee.ImagePath = fileName;
                        }
                        else
                        {
                            employee.ImagePath = detail.ImagePath;
                            employee.ID = detail.EmployeeID;
                            employee.UserNo = Convert.ToInt32(textUserNo.Text);
                            employee.Name = txtName.Text;
                            employee.SurName = txtSurname.Text;
                            employee.isAdmin = chAdmin.Checked;
                            employee.Password = txtPassword.Text;
                            employee.Address = txtAddress.Text;
                            employee.Birthday = dateTimePicker1.Value;
                            employee.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                            employee.PositionID = Convert.ToInt32(cmbPosition.SelectedValue);
                            employee.Salary = Convert.ToInt32(txtSalary.Text);
                            EmployeeBLL.UpdateEmployee(employee);
                            MessageBox.Show("Employee was updated");
                                this.Close();
                        }
                    }
                }
                
            }
        }

        private void textUserNo_TextChanged(object sender, EventArgs e)
        {

        }

        bool isUnique = false;
        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (textUserNo.Text.Trim() == "")
                MessageBox.Show("User no is empty");
            else
            {
                isUnique = EmployeeBLL.isUnique(Convert.ToInt32(textUserNo.Text));
                if (!isUnique)
                    MessageBox.Show("This user no is used by another employee, please change");
                else
                    MessageBox.Show("This user no is usable");
            }
        }

        private void cmbPosition_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSalary_TextChanged(object sender, EventArgs e)
        {

        }

        private void textPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
