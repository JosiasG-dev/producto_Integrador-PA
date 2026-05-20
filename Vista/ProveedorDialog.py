from PyQt6.QtWidgets import QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton, QMessageBox, QWidget
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont
from Vista.Estilos import Estilos
from Modelo.Proveedor import Proveedor

class ProveedorDialog(QDialog):
    def __init__(self, parent, p, ctrl):
        super().__init__(parent)
        self.ctrl = ctrl
        self.proveedor = p
        self.setWindowTitle("Nuevo Proveedor" if p is None else "Editar Proveedor")
        self.setModal(True)
        self.construir()
        if p:
            self.cargar(p)

    def construir(self):
        self.setFixedSize(500, 500)
        panel = QWidget(self)
        panel.resize(500, 500)
        panel.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")

        layout = QVBoxLayout(panel)
        layout.setContentsMargins(32, 28, 32, 28)
        layout.setSpacing(10)

        tit = QLabel("NUEVO PROVEEDOR" if self.proveedor is None else "EDITAR PROVEEDOR")
        tit.setFont(Estilos.FUENTE_TITULO)
        layout.addWidget(tit)
        layout.addSpacing(10)

        layout.addWidget(self.lbl("Empresa / Razón Social"))
        self.txtNombre = self.campo()
        layout.addWidget(self.txtNombre)

        layout.addWidget(self.lbl("Nombre de Contacto"))
        self.txtContacto = self.campo()
        layout.addWidget(self.txtContacto)

        layout.addWidget(self.lbl("Teléfono"))
        self.txtTelefono = self.campo()
        layout.addWidget(self.txtTelefono)

        layout.addWidget(self.lbl("Email Corporativo"))
        self.txtEmail = self.campo()
        layout.addWidget(self.txtEmail)

        layout.addWidget(self.lbl("Dirección / Matriz"))
        self.txtDireccion = self.campo()
        layout.addWidget(self.txtDireccion)

        layout.addSpacing(14)

        btnPanel = QHBoxLayout()
        btnPanel.addStretch()

        if self.proveedor:
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
        self.ctrl.eliminarProveedor(self.proveedor.getId())
        self.accept()

    def cargar(self, p):
        self.txtNombre.setText(p.getNombre())
        self.txtContacto.setText(p.getContacto())
        self.txtTelefono.setText(p.getTelefono())
        self.txtEmail.setText(p.getEmail())
        self.txtDireccion.setText(p.getDireccion())

    def guardar(self):
        nombre = self.txtNombre.text().strip().upper()
        if not nombre:
            QMessageBox.critical(self, "Error", "El nombre es obligatorio.")
            return

        id_prov = self.proveedor.getId() if self.proveedor else self.ctrl.generarIdProveedor()
        p = Proveedor(id_prov, nombre, self.txtContacto.text().strip(), self.txtTelefono.text().strip(),
                     self.txtEmail.text().strip(), self.txtDireccion.text().strip(), True)
        self.ctrl.guardarProveedor(p)
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
