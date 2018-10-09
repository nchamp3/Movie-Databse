//
// Netflix Database Application using N-Tier Design.
//
// <<Neil Champakara>>
// U. of Illinois, Chicago
// CS341, Spring 2018
// Project 08
//




using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetflixApp
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

        private bool fileExists(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                string msg = string.Format("Input file not found: '{0}'",
                  filename);

                MessageBox.Show(msg);
                return false;
            }

            // exists!
            return true;
        }

        

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void showMovies(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            SqlConnection db;
            String version = "MSSQLLocalDB";
            String filename = this.dbname.Text;
            String connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", version, filename);
            db = new SqlConnection(connectionInfo);
            db.Open();

            string sql = string.Format(@"SELECT MovieName FROM Movies ORDER BY MovieName ASC;");


            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            cmd.CommandText = sql;
            adapter.Fill(ds);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string msg = string.Format("{0}", Convert.ToString(row["MovieName"]));
                this.listBox1.Items.Add(msg);
            }
            //this.listBox1.SelectedIndex = 0;
            db.Close();
        }

        private void showUsers(object sender, EventArgs e)
        {
            String dbfilename = this.dbname.Text; // get DB name from text box:          
            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if(biztier.TestConnection())
            {
                IReadOnlyList<BusinessTier.User> users = biztier.GetAllNamedUsers();
                this.listBox2.Items.Clear();
                foreach (var j in users)
                {
                    this.listBox2.Items.Add(j.UserName);
                }
            }
            else
            {
                MessageBox.Show("Connection could not be established");
                return;
            }
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            showMovies(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();
            this.textBox1.Clear();
            this.textBox4.Clear();
            this.textBox6.Clear();
            this.textBox9.Clear();
            this.textBox10.Clear();
            
        }

        private void dbname_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection db;
            String version = "MSSQLLocalDB";
            String filename = this.dbname.Text;
            String connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", version, filename);
            db = new SqlConnection(connectionInfo);
            db.Open();
            string mname = this.listBox1.Text;
            mname = mname.Replace("'", "''");
            string sql = string.Format(@"SELECT MovieID FROM Movies WHERE Movies.MovieName = '{0}';", mname);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.CommandText = sql;
            object result = cmd.ExecuteScalar();
            int id;
            if(result == null)
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(result);
            }
            sql = string.Format(@"SELECT SUM(Rating) FROM Reviews, Movies WHERE Movies.MovieName = '{0}' AND Movies.MovieID = Reviews.MovieID;", mname);
            cmd.CommandText = sql;
            result = cmd.ExecuteScalar();
            int Totalrating;
            if (result == null)
            {
                Totalrating = 0;
            }
            else if(result == DBNull.Value)
            {
                Totalrating = 0; 
            }
            else
            {
                Totalrating = Convert.ToInt32(result);
            }
            sql = string.Format(@"SELECT COUNT(Rating) FROM Reviews, Movies WHERE Movies.MovieName = '{0}' AND Movies.MovieID = Reviews.MovieID;", mname);
            cmd.CommandText = sql;
            result = cmd.ExecuteScalar();
            int Numrating;
            if (result == null)
            {
                Numrating = 0;
            }
            else
            {
                Numrating = Convert.ToInt32(result);
            }


            this.textBox9.Text = Convert.ToString(id);
            this.textBox10.Text = Convert.ToString((double) Totalrating/ Numrating);
            db.Close();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            showUsers(sender, e);

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            String dbfilename = this.dbname.Text; // get DB name from text box: 
            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if(biztier.TestConnection())
            {
                string uname = this.listBox2.Text;

                BusinessTier.User userdetails = biztier.GetNamedUser(uname);

                this.textBox1.Text = Convert.ToString(userdetails.UserID);
                this.textBox4.Text = userdetails.Occupation;
            }
            else
            {
                MessageBox.Show("Connection could not be established");
                return;
            }
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string mname;
            int mid;
            
            String dbfilename = this.dbname.Text; // get DB name from text box: 
            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (biztier.TestConnection())
            {
                if (this.listBox1.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a movie");
                    return;
                }
                else
                {
                    this.listBox2.Items.Clear();
                    mname = this.listBox1.Text;
                    mid = System.Int32.Parse(this.textBox9.Text);
                    BusinessTier.MovieDetail details = biztier.GetMovieDetail(mid);


                    this.listBox2.Items.Add(mname);
                    this.listBox2.Items.Add("");
                    foreach (var j in details.Reviews)
                    {
                        this.listBox2.Items.Add(j.UserID + " : " + j.Rating);
                    }

                }
            }
            else
            {
                MessageBox.Show("Connection could not be established");
                return;
            }

           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String dbfilename = this.dbname.Text; // get DB name from text box: 
            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);

            if (biztier.TestConnection())
            {
                int mid, uid, rating;
                if(this.listBox1.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a movie");
                    return;
                }
                else
                {
                    mid = Convert.ToInt32(this.textBox9.Text);
                }
                if(this.listBox2.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a user");
                    return;
                }
                else
                {
                    uid = Convert.ToInt32(this.textBox1.Text);
                }
                if(this.textBox6.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a rating");
                    return;
                }
                else
                {
                    rating = Convert.ToInt32(this.textBox6.Text);
                }         

                if (rating < 1 || rating > 5)
                {
                    MessageBox.Show("Rating must range from 1-5");
                    return;
                }
                else
                {
                    BusinessTier.Review newreview = biztier.AddReview(mid, uid, rating);
                    if (newreview != null)
                    {
                        MessageBox.Show("Success!");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Failed to add review");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Connection could not be established");
                return;
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            showMovies(sender, e);
            showUsers(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String dbfilename = this.dbname.Text; // get DB name from text box: 
            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (biztier.TestConnection())
            {
                BusinessTier.MovieDetail details;
                if (this.listBox1.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a movie");
                    return;
                }
                else
                {
                    details = biztier.GetMovieDetail(Convert.ToInt32(this.textBox9.Text));
                }
                if (details == null)
                {
                    MessageBox.Show("Movie not found");
                    return;
                }
                else
                {
                    IReadOnlyList<BusinessTier.Review> reviews = details.Reviews;
                    int count = 0;
                    int total = 0;
                    this.listBox2.Items.Clear();
                    this.listBox2.Items.Add(this.listBox1.Text);
                    this.listBox2.Items.Add("");
                    for (int i = 5; i >= 1; i--)
                    {
                        foreach (var j in reviews)
                        {
                            if (j.Rating == i)
                                count++;
                        }
                        this.listBox2.Items.Add(i + ": " + count + "\n");
                        total += count;
                        count = 0;
                    }
                    this.listBox2.Items.Add("");
                    this.listBox2.Items.Add("Total: " + total + "\n");
                }
            }
            else
            {
                MessageBox.Show("Connection could not be established");
                return;
            }
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(this.textBox11.Text.Length == 0)
            {
                MessageBox.Show("Please enter a value");
                return;
            }
            else
            {
                String dbfilename = this.dbname.Text; // get DB name from text box: 
                BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
                if (biztier.TestConnection())
                {
                    IReadOnlyList<BusinessTier.Movie> topReviews = biztier.GetTopMoviesByAvgRating(Convert.ToInt32(this.textBox11.Text));
                    BusinessTier.MovieDetail details;
                    this.listBox1.Items.Clear();
                    foreach (var j in topReviews)
                    {
                        details = biztier.GetMovieDetail(j.MovieID);
                        this.listBox1.Items.Add(j.MovieName + ": " + details.AvgRating + "\n");
                    }
                }
                else
                {
                    MessageBox.Show("Connection could not be established");
                    return;
                }
                
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            String uname;
            String dbfilename = this.dbname.Text; // get DB name from text box: 
            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (biztier.TestConnection())
            {
                if (this.listBox2.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a user");
                    return;
                }
                else
                {
                    uname = this.listBox2.Text;
                    BusinessTier.UserDetail userdetails = biztier.GetUserDetail(System.Int32.Parse(this.textBox1.Text));
                    BusinessTier.Movie mDetails;
                    this.listBox1.Items.Clear();
                    this.listBox1.Items.Add(uname);
                    this.listBox1.Items.Add("");

                    foreach (var j in userdetails.Reviews)
                    {
                        mDetails = biztier.GetMovie(j.MovieID);
                        this.listBox1.Items.Add(mDetails.MovieName + " -> " + j.Rating);
                    }
                }
            }
            else
            {
                MessageBox.Show("Connection could not be established");
                return;
            }            
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
