from PyQt6.QtWidgets import QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton, QMessageBox, QComboBox, QSpinBox, QWidget
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont
from Vista.Estilos import Estilos
from Modelo.Usuario import Usuario

class UsuarioDialog(QDialog):
    def __init__(self, parent, u, ctrl):
        super().__init__(parent)
        self.ctrl = ctrl
        self.usuario = u
        self.setWindowTitle("Alta Personal" if u is None else "Editar Personal")
        self.setModal(True)
        self.construir()
        if u:
            self.cargar(u)

    def construir(self):
        self.setFixedSize(480, 520)
        panel = QWidget(self)
        panel.resize(480, 520)
        panel.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")

        layout = QVBoxLayout(panel)
        layout.setContentsMargins(32, 28, 32, 28)
        layout.setSpacing(10)

        tit = QLabel("ALTA PERSONAL" if self.usuario is None else "EDITAR PERSONAL")
        tit.setFont(Estilos.FUENTE_TITULO)
        layout.addWidget(tit)
        layout.addSpacing(10)

        layout.addWidget(self.lbl("Nombre Completo"))
        self.txtNombre = self.campo()
        layout.addWidget(self.txtNombre)

        layout.addWidget(self.lbl("Rol en el Sistema"))
        self.cmbRol = QComboBox()
        self.cmbRol.addItems(["CAJERO", "ADMINISTRADOR"])
        self.cmbRol.setFont(Estilos.FUENTE_BOLD)
        self.cmbRol.setFixedHeight(42)
        layout.addWidget(self.cmbRol)

        layout.addWidget(self.lbl("Usuario de Acceso (Red)"))
        self.txtUsername = self.campo()
        layout.addWidget(self.txtUsername)

        layout.addWidget(self.lbl("Contraseña"))
        self.txtPassword = self.campo()
        layout.addWidget(self.txtPassword)

        row = QHBoxLayout()
        leftRow = QVBoxLayout()
        leftRow.addWidget(self.lbl("Edad"))
        self.spEdad = QSpinBox()
        self.spEdad.setRange(16, 70)
        self.spEdad.setValue(18)
        self.spEdad.setFont(Estilos.FUENTE_BOLD)
        self.spEdad.setFixedHeight(42)
        leftRow.addWidget(self.spEdad)

        rightRow = QVBoxLayout()
        rightRow.addWidget(self.lbl("Sexo"))
        self.cmbSexo = QComboBox()
        self.cmbSexo.addItems(["Femenino", "Masculino", "Otro"])
        self.cmbSexo.setFont(Estilos.FUENTE_BOLD)
        self.cmbSexo.setFixedHeight(42)
        rightRow.addWidget(self.cmbSexo)

        row.addLayout(leftRow)
        row.addLayout(rightRow)
        layout.addLayout(row)
        layout.addSpacing(14)

        btnPanel = QHBoxLayout()
        btnPanel.addStretch()

        if self.usuario and self.usuario.getId() != self.ctrl.getUsuarioActivo().getId():
            btnElim = Estilos.botonPeligro("Eliminar")
            btnElim.setFixedSize(110, 42)
            btnElim.clicked.connect(self.eliminar)
            btnPanel.addWidget(btnElim)

        btnCancelar = Estilos.botonSecundario("Cancelar")
        btnCancelar.setFixedSize(110, 42)
        btnCancelar.clicked.connect(self.reject)
        
        btnGuardar = Estilos.botonPrimario("Guardar")
        btnGuardar.setFixedSize(120, 42)
        btnGuardar.clicked.connect(self.guardar)

        btnPanel.addWidget(btnCancelar)
        btnPanel.addWidget(btnGuardar)
        
        layout.addLayout(btnPanel)

    def eliminar(self):
        self.ctrl.eliminarUsuario(self.usuario.getId())
        self.accept()

    def cargar(self, u):
        self.txtNombre.setText(u.getNombre())
        self.cmbRol.setCurrentText(u.getRol())
        self.txtUsername.setText(u.getUsername())
        self.txtPassword.setText(u.getPassword())
        self.spEdad.setValue(u.getEdad())
        self.cmbSexo.setCurrentText(u.getSexo())

    def guardar(self):
        nombre = self.txtNombre.text().strip()
        username = self.txtUsername.text().strip()
        password = self.txtPassword.text().strip()

        if not nombre or not username or not password:
            QMessageBox.critical(self, "Error", "Nombre, usuario y contraseña son obligatorios.")
            return

        id_usu = self.usuario.getId() if self.usuario else self.ctrl.generarId()
        u = Usuario(id_usu, username, password, self.cmbRol.currentText(), nombre, self.spEdad.value(), self.cmbSexo.currentText())
        self.ctrl.guardarUsuario(u)
        self.accept()

    def lbl(self, t):
        l = QLabel(t.upper())
        l.setFont(Estilos.FUENTE_XS)
        l.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        return l

    def campo(self):
        tf = QLineEdit()
        tf.setFont(Estilos.FUENTE_BOLD)
        tf.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 10px 12px;
            }}
        """)
        tf.setFixedHeight(44)
        return tf
