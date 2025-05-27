namespace Doodle_Jump
{
    partial class Main_menu_form
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_new_game = new System.Windows.Forms.Button();
            this.btn_continue_game = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_new_game
            // 
            this.btn_new_game.Location = new System.Drawing.Point(174, 237);
            this.btn_new_game.Name = "btn_new_game";
            this.btn_new_game.Size = new System.Drawing.Size(158, 23);
            this.btn_new_game.TabIndex = 0;
            this.btn_new_game.Text = "Новая игра";
            this.btn_new_game.UseVisualStyleBackColor = true;
            this.btn_new_game.Click += new System.EventHandler(this.btn_new_game_Click);
            // 
            // btn_continue_game
            // 
            this.btn_continue_game.Location = new System.Drawing.Point(360, 237);
            this.btn_continue_game.Name = "btn_continue_game";
            this.btn_continue_game.Size = new System.Drawing.Size(212, 23);
            this.btn_continue_game.TabIndex = 1;
            this.btn_continue_game.Text = "Продолжить игру";
            this.btn_continue_game.UseVisualStyleBackColor = true;
            this.btn_continue_game.Click += new System.EventHandler(this.btn_continue_game_Click);
            // 
            // Main_menu_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_continue_game);
            this.Controls.Add(this.btn_new_game);
            this.Name = "Main_menu_form";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_new_game;
        private System.Windows.Forms.Button btn_continue_game;
    }
}

