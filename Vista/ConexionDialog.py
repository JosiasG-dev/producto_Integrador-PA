from PyQt6.QtWidgets import QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QComboBox, QPushButton, QMessageBox, QWidget
from PyQt6.QtCore import Qt
from ConexionBD.Conexion import Conexion, TipoMotor, TipoAutenticacion

class ConexionDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Configuracion de Conexion")
        self.setModal(True)
        self.confirmado = False
        self.construirUI()
        self.setFixedSize(400, 450)

    def construirUI(self):
        layout = QVBoxLayout(self)
        layout.setContentsMargins(24, 20, 24, 20)
        layout.setSpacing(10)

        titulo = QLabel("Selecciona el motor de base de datos")
        font = titulo.font()
        font.setBold(True)
        font.setPointSize(14)
        titulo.setFont(font)
        layout.addWidget(titulo)

        def add_row(label_text, widget):
            row = QHBoxLayout()
            lbl = QLabel(label_text)
            lbl.setFixedWidth(100)
            row.addWidget(lbl)
            row.addWidget(widget)
            layout.addLayout(row)
            return lbl

        self.comboMotor = QComboBox()
        self.comboMotor.addItems(["MySQL"])
        self.comboMotor.setEnabled(False)
        add_row("Motor:", self.comboMotor)

        self.campoHost = QLineEdit("localhost")
        add_row("Host:", self.campoHost)

        self.campoPuerto = QLineEdit("3306")
        self.labelPuerto = add_row("Puerto:", self.campoPuerto)

        self.campoInstancia = QLineEdit("1433")
        self.labelInstancia = add_row("Puerto TCP:", self.campoInstancia)

        self.campoBase = QLineEdit("corporativo_pos")
        add_row("Base de datos:", self.campoBase)

        self.campoUsuario = QLineEdit("root")
        self.labelUsuario = add_row("Usuario:", self.campoUsuario)

        self.campoContrasena = QLineEdit("2306")
        self.campoContrasena.setEchoMode(QLineEdit.EchoMode.Password)
        self.labelContrasena = add_row("Contrasena:", self.campoContrasena)

        self.comboMotor.currentIndexChanged.connect(self.actualizarVista)

        btnLayout = QHBoxLayout()
        btnLayout.addStretch()
        
        btnSalir = QPushButton("Salir")
        btnSalir.clicked.connect(self.reject)
        
        btnConectar = QPushButton("Conectar")
        btnConectar.clicked.connect(self.intentarConexion)
        
        btnLayout.addWidget(btnSalir)
        btnLayout.addWidget(btnConectar)
        
        layout.addLayout(btnLayout)
        self.actualizarVista()

    def actualizarVista(self):
        self.labelInstancia.setVisible(False)
        self.campoInstancia.setVisible(False)
        self.labelPuerto.setVisible(True)
        self.campoPuerto.setVisible(True)
        self.labelUsuario.setVisible(True)
        self.campoUsuario.setVisible(True)
        self.labelContrasena.setVisible(True)
        self.campoContrasena.setVisible(True)
        self.campoPuerto.setText("3306")
        self.campoUsuario.setText("root")

    def intentarConexion(self):
        host = self.campoHost.text().strip()
        base = self.campoBase.text().strip()
        puerto = self.campoPuerto.text().strip()
        usr = self.campoUsuario.text().strip()
        pass_ = self.campoContrasena.text()

        if not host or not base or not puerto or not usr:
            QMessageBox.warning(self, "Aviso", "Completa todos los campos.")
            return

        Conexion.configurar(TipoMotor.MYSQL, TipoAutenticacion.CREDENCIALES, host, puerto, base, usr, pass_)

        if Conexion.getConexion() is not None:
            self.confirmado = True
            self.accept()
        else:
            QMessageBox.critical(self, "Error de Conexion", "No se pudo conectar.\nRevisa los datos e intenta de nuevo.")
            Conexion.cerrar()

    def fueConfirmado(self):
        return self.confirmado
