using System.ComponentModel;

namespace ClientDataParser;

public partial class MainForm : Form
{ 
    private IContainer components = null;
    private DataGridView dataGridView;
    private Button btnImport;
    private Button btnSave; 
    
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    } 
    
    private void InitializeComponent() 
    { 
        this.dataGridView = new DataGridView(); 
        this.btnImport = new Button(); 
        this.btnSave = new Button(); 
        ((ISupportInitialize)(this.dataGridView)).BeginInit(); 
        this.SuspendLayout();
        
        #region Инициализация элементов управления
        InitializeDataGridView(); 
        InitializeButtons();
        #endregion
        
        #region Настройки формы
        this.ClientSize = new Size(784, 461); 
        this.Controls.Add(this.btnImport); 
        this.Controls.Add(this.btnSave); 
        this.Controls.Add(this.dataGridView); 
        this.Name = "MainForm"; 
        this.Text = "Управление клиентами"; 
        this.BackColor = Color.White;
        #endregion
        
        ((ISupportInitialize)(this.dataGridView)).EndInit(); 
        this.ResumeLayout(false); 
    } 
    
    private void InitializeDataGridView() 
    {
        #region Настройки внешнего вида
        this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridView.Dock = DockStyle.Fill; // Изменено на DockStyle.Fill
        this.dataGridView.Location = new Point(0, 0);
        this.dataGridView.Name = "dataGridView";
        this.dataGridView.TabIndex = 0;
        this.dataGridView.BackgroundColor = Color.White;
        this.dataGridView.BorderStyle = BorderStyle.None;
        this.dataGridView.GridColor = Color.LightGray;
        #endregion
        
        #region Слушатели событий
        this.dataGridView.CellBeginEdit += new DataGridViewCellCancelEventHandler(this.dataGridView_CellBeginEdit); 
        this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
        #endregion
        
        #region Настройки взаимодействия
        this.dataGridView.ReadOnly = false; 
        this.dataGridView.AllowUserToAddRows = false;
        #endregion
        
        #region Настройки стилей заголовков
        this.dataGridView.EnableHeadersVisualStyles = false; 
        this.dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; 
        this.dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(34, 36, 49); 
        this.dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; 
        this.dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        #endregion
        
        #region Настройки стилей ячеек
        this.dataGridView.DefaultCellStyle.BackColor = Color.FromArgb(244, 244, 244); 
        this.dataGridView.DefaultCellStyle.ForeColor = Color.Black; 
        this.dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 9); 
        this.dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(89, 151, 249); 
        this.dataGridView.DefaultCellStyle.SelectionForeColor = Color.White; 
        this.dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single; 
        this.dataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        #endregion
    } 
    private void InitializeButtons() 
    {
        #region btnImport
        this.btnImport.Dock = DockStyle.Bottom; 
        this.btnImport.Size = new Size(784, 40); // Увеличьте высоту кнопки
        this.btnImport.TabIndex = 1; 
        this.btnImport.Text = "Import"; 
        this.btnImport.UseVisualStyleBackColor = true; 
        this.btnImport.Click += new EventHandler(this.btnImport_Click); 
        this.btnImport.Font = new Font("Segoe UI", 12, FontStyle.Bold); // Увеличьте шрифт
        this.btnImport.BackColor = Color.FromArgb(34, 36, 49);
        this.btnImport.ForeColor = Color.White;
        this.btnImport.FlatStyle = FlatStyle.Flat; // Сделайте стиль плоским
        this.btnImport.FlatAppearance.BorderSize = 1; // Добавьте границу
        this.btnImport.FlatAppearance.BorderColor = Color.LightGray; // Цвет границы
        #endregion

        #region btnSave
        this.btnSave.Dock = DockStyle.Bottom;
        this.btnSave.Size = new Size(784, 40); // Увеличьте высоту кнопки
        this.btnSave.TabIndex = 2;
        this.btnSave.Text = "Save Changes";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new EventHandler(this.btnSave_Click);
        this.btnSave.Font = new Font("Segoe UI", 12, FontStyle.Bold); // Увеличьте шрифт
        this.btnSave.BackColor = Color.FromArgb(34, 36, 49);
        this.btnSave.ForeColor = Color.White;
        this.btnSave.FlatStyle = FlatStyle.Flat; // Сделайте стиль плоским
        this.btnSave.FlatAppearance.BorderSize = 1; // Добавьте границу
        this.btnSave.FlatAppearance.BorderColor = Color.LightGray; // Цвет границы
        #endregion
    }
    
    #region Обработчики событий кнопок
    private void btnSave_Click(object sender, EventArgs e)
    {
        using (var context = new AppDbContext())
        {
            context.SaveChanges();
            MessageBox.Show("Изменения сохранены.");
        }
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Excel Files|*.xlsx";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImportFromExcel(openFileDialog.FileName);
                LoadClients();
            }
        }
    }
    #endregion
}

