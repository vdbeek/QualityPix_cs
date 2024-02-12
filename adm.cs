using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QP
{
    public partial class adm : Form
    {
        private cSQL cSql;

        private DataTable initialMovies;
        private DataTable initialReviews;
        private DataTable initialUsers;
        
        private DataGridView dgvMovies;
        private DataGridView dgvReviews;
        private DataGridView dgvUsers;
        public adm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // sql
            cSql = new cSQL();

            // close button
            Size = new Size(1000, 500);
            Button btnClose = new Button();
            btnClose.Name = @"btnClose";
            btnClose.Text = @"Close";
            btnClose.Location = new Point(ClientRectangle.Location.X+ClientRectangle.Width-btnClose.Width,ClientRectangle.Location.Y+ClientRectangle.Height-btnClose.Height);
            btnClose.MouseClick += BtnCloseOnMouseClick;

            btnClose.Anchor = AnchorStyles.Bottom & AnchorStyles.Right;
            
            Controls.Add(btnClose);
            
            initialMovies = cSql.Query("select * from db_movies").Tables[0];
            initialReviews = cSql.Query("select * from db_reviews").Tables[0];
            initialUsers = cSql.Query("select * from db_users").Tables[0];
            
            //dgv1
            var editMode = DataGridViewEditMode.EditOnEnter;
            var sizeMode = DataGridViewAutoSizeRowsMode.AllCells;
            
            dgvMovies = new DataGridView();
            dgvMovies.Size = new Size(800, 300);
            dgvMovies.EditMode = editMode;
            dgvMovies.AutoSizeRowsMode = sizeMode;
            dgvMovies.DataSource = initialMovies;
            
            Controls.Add(dgvMovies);

            Button restoreMovies = new Button();
            restoreMovies.Text = @"undo all";
            restoreMovies.Location =
                new Point(dgvMovies.ClientRectangle.X + dgvMovies.ClientRectangle.Width - restoreMovies.Width - 100,
                    dgvMovies.ClientRectangle.Y + dgvMovies.ClientRectangle.Height + restoreMovies.Height / 2);

            restoreMovies.MouseClick += RestoreMoviesOnMouseClick;

            Controls.Add(restoreMovies);

            Button commitMovies = new Button();
            commitMovies.Text = @"commit";
            commitMovies.Location = new Point(restoreMovies.Location.X + restoreMovies.ClientRectangle.Width + 10,
                restoreMovies.Location.Y);
            
            commitMovies.MouseClick += CommitMoviesOnMouseClick;
            
            Controls.Add(commitMovies);

            dgvReviews = new DataGridView();
            dgvReviews.Size = dgvMovies.Size;
            dgvReviews.Location = new Point(dgvMovies.Location.X, restoreMovies.Location.Y + restoreMovies.Size.Height + 10);
            dgvReviews.EditMode = editMode;
            dgvReviews.AutoSizeRowsMode = sizeMode;
            dgvReviews.DataSource = initialReviews;
            
            Controls.Add(dgvReviews);
            
            Button restoreReviews = new Button();
            restoreReviews.Text = @"undo all";
            restoreReviews.Location =
                new Point(dgvReviews.Location.X + dgvReviews.ClientRectangle.Width - restoreReviews.Width - 100,
                    dgvReviews.Location.Y+dgvReviews.ClientRectangle.Height+10);
            
            restoreReviews.MouseClick += RestoreReviewsOnMouseClick;

            Controls.Add(restoreReviews);

            Button commitReviews = new Button();
            commitReviews.Text = @"commit";
            commitReviews.Location = new Point(restoreReviews.Location.X + restoreReviews.ClientRectangle.Width + 10,
                restoreReviews.Location.Y);
            
            commitReviews.MouseClick += CommitReviewsOnMouseClick;
            
            Controls.Add(commitReviews);

            dgvUsers = new DataGridView();
            dgvUsers.Size = dgvMovies.Size;
            dgvUsers.Location = new Point(dgvMovies.Location.X,restoreReviews.Location.Y+restoreReviews.ClientRectangle.Height+10);
            dgvUsers.DataSource = initialUsers;
            
            Controls.Add(dgvUsers);
            
            Button restoreUsers = new Button();
            restoreUsers.Text = @"undo all";
            restoreUsers.Location =
                new Point(dgvUsers.Location.X + dgvUsers.ClientRectangle.Width - restoreUsers.Width - 100,
                    dgvUsers.Location.Y+dgvUsers.ClientRectangle.Height+10);
            
            restoreUsers.MouseClick += RestoreUsersOnMouseClick;

            Controls.Add(restoreUsers);

            Button commitUsers = new Button();
            commitUsers.Text = @"commit";
            commitUsers.Location = new Point(restoreUsers.Location.X + restoreUsers.ClientRectangle.Width + 10,
                restoreUsers.Location.Y);
            
            commitUsers.MouseClick += CommitUsersOnMouseClick;
            
            Controls.Add(commitUsers);
        }

        private void RestoreMoviesOnMouseClick(object sender, MouseEventArgs e)
        {
            dgvMovies.DataSource = initialMovies;
        }
        private void RestoreReviewsOnMouseClick(object sender, MouseEventArgs e)
        {
            dgvReviews.DataSource = initialReviews;
        }
        private void RestoreUsersOnMouseClick(object sender, MouseEventArgs e)
        {
            dgvUsers.DataSource = initialUsers;
        }
        
        private void CommitMoviesOnMouseClick(object sender, MouseEventArgs e)
        {
            using (var sql = cSql.Connect())
            {
                using (var bulk = new SqlBulkCopy(sql))
                {
                    bulk.BatchSize = 500;
                    bulk.NotifyAfter = 1000;
                    bulk.DestinationTableName = "db_movies";
                    var data = ((DataTable)dgvMovies.DataSource);
                    SqlCommand cmd = new SqlCommand("delete from db_movies", sql);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    bulk.WriteToServer(data);
                    MessageBox.Show("Database overwritten with new data");
                }
            }
        }
        
        private void CommitReviewsOnMouseClick(object sender, MouseEventArgs e)
        {
            using (var sql = cSql.Connect())
            {
                using (var bulk = new SqlBulkCopy(sql))
                {
                    bulk.BatchSize = 500;
                    bulk.NotifyAfter = 1000;
                    bulk.DestinationTableName = "db_reviews";
                    var data = ((DataTable)dgvReviews.DataSource);
                    SqlCommand cmd = new SqlCommand("delete from db_reviews", sql);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    bulk.WriteToServer(data);
                    MessageBox.Show("Database overwritten with new data");
                }
            }
        }

        private void CommitUsersOnMouseClick(object sender, MouseEventArgs e)
        {
            using (var sql = cSql.Connect())
            {
                using (var bulk = new SqlBulkCopy(sql))
                {
                    bulk.BatchSize = 500;
                    bulk.NotifyAfter = 1000;
                    bulk.DestinationTableName = "db_users";
                    var data = ((DataTable)dgvReviews.DataSource);
                    SqlCommand cmd = new SqlCommand("delete from db_users", sql);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    bulk.WriteToServer(data);
                    MessageBox.Show("Database overwritten with new data");
                }
            }
        }
        
        private void BtnCloseOnMouseClick(object sender, MouseEventArgs e)
        {
            Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            var btn = Controls.Find(@"btnClose", true);
            if (btn.Length > 0)
            {
                if (btn[0] != null)
                {
                    btn[0].Location = new Point(ClientRectangle.Location.X + ClientRectangle.Width - btn[0].Width,
                        ClientRectangle.Location.Y + ClientRectangle.Height - btn[0].Height);
                }
            }
            
        }
    }
}