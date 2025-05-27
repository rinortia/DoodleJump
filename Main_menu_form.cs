using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doodle_Jump
{
    public partial class Main_menu_form : Form
    {
        public Main_menu_form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_new_game_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm(); // создаём форму игры
            gameForm.Show();                    // показываем
            this.Hide();                        // скрываем меню
        }

        private void btn_continue_game_Click(object sender, EventArgs e)
        {

        }
    }
}
