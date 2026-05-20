from PyQt6.QtWidgets import QMainWindow, QWidget, QLabel, QLineEdit, QPushButton, QVBoxLayout
from PyQt6.QtCore import Qt, QSize
from PyQt6.QtGui import QFont
from Vista.Estilos import Estilos

class LoginVista:
    def __init__(self, app):
        self.app = app
        self.frame = QMainWindow()
        self.construir()

    def construir(self):
        self.frame.setWindowTitle("")
        self.frame.setFixedSize(460, 520)

        fondo = QWidget()
        fondo.setStyleSheet(f"background-color: {Estilos.BG_OSCURO.name()};")

        tarjeta = QWidget(fondo)
        tarjeta.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        tarjeta.setGeometry(44, 30, 372, 440)

        icono = QLabel("PUNTO DE VENTA", tarjeta)
        icono.setFont(QFont("SansSerif", 18, QFont.Weight.Bold))
        icono.setStyleSheet(f"color: {Estilos.INDIGO.name()}; border: none;")
        icono.setAlignment(Qt.AlignmentFlag.AlignCenter)
        icono.setGeometry(0, 24, 372, 30)

        titulo = QLabel("Terminal Corporativa", tarjeta)
        titulo.setFont(Estilos.FUENTE_TITULO)
        titulo.setStyleSheet(f"color: {Estilos.TEXTO_PRINCIPAL.name()}; border: none;")
        titulo.setAlignment(Qt.AlignmentFlag.AlignCenter)
        titulo.setGeometry(0, 60, 372, 28)

        sub = QLabel("SISTEMA POS", tarjeta)
        sub.setFont(Estilos.FUENTE_XS)
        sub.setStyleSheet(f"color: {Estilos.INDIGO.name()}; border: none;")
        sub.setAlignment(Qt.AlignmentFlag.AlignCenter)
        sub.setGeometry(0, 92, 372, 16)

        lblUsr = QLabel("USUARIO", tarjeta)
        lblUsr.setFont(Estilos.FUENTE_XS)
        lblUsr.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        lblUsr.setGeometry(36, 132, 300, 14)

        self.txtUsuario = Estilos.campo()
        self.txtUsuario.setParent(tarjeta)
        self.txtUsuario.setGeometry(36, 150, 300, 44)

        lblPass = QLabel("CONTRASEÑA", tarjeta)
        lblPass.setFont(Estilos.FUENTE_XS)
        lblPass.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        lblPass.setGeometry(36, 206, 300, 14)

        self.txtPassword = Estilos.campo()
        self.txtPassword.setParent(tarjeta)
        self.txtPassword.setEchoMode(QLineEdit.EchoMode.Password)
        self.txtPassword.setGeometry(36, 224, 300, 44)

        self.lblError = QLabel(" ", tarjeta)
        self.lblError.setFont(Estilos.FUENTE_SMALL)
        self.lblError.setStyleSheet(f"color: {Estilos.ROSE.name()}; border: none;")
        self.lblError.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.lblError.setGeometry(36, 276, 300, 16)

        btnEntrar = Estilos.botonPrimario("ENTRAR AL SISTEMA")
        btnEntrar.setParent(tarjeta)
        btnEntrar.setGeometry(36, 302, 300, 48)
        btnEntrar.clicked.connect(self.intentarLogin)

        self.txtPassword.returnPressed.connect(self.intentarLogin)

        self.frame.setCentralWidget(fondo)

    def intentarLogin(self):
        user = self.txtUsuario.text().strip()
        pass_ = self.txtPassword.text().strip()
        u = self.app.autenticar(user, pass_)
        if u is not None:
            self.lblError.setText(" ")
            self.app.onLoginExitoso(u)
        else:
            self.lblError.setText("Credenciales invalidas")

    def mostrar(self):
        self.frame.show()

    def ocultar(self):
        self.frame.hide()

    def limpiar(self):
        self.txtUsuario.setText("")
        self.txtPassword.setText("")
        self.lblError.setText(" ")
