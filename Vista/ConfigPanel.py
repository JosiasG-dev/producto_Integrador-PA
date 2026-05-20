from PyQt6.QtWidgets import QWidget, QVBoxLayout, QHBoxLayout, QLabel, QPushButton, QLineEdit, QScrollArea, QMessageBox, QInputDialog
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont
from Vista.Estilos import Estilos
from Modelo.ConfiguracionTienda import ConfiguracionTienda

class ConfigPanel(QWidget):
    def __init__(self, app):
        super().__init__()
        self.app = app
        self.construir()

    def construir(self):
        self.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")
        layout = QVBoxLayout(self)
        layout.setContentsMargins(24, 24, 24, 24)
        layout.setSpacing(16)

        header = QWidget()
        header.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        h_layout = QHBoxLayout(header)
        h_layout.setContentsMargins(24, 20, 24, 20)
        tit = QLabel("⚙️  SISTEMA  —  Identidad Fiscal")
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setStyleSheet("border: none;")
        h_layout.addWidget(tit)
        layout.addWidget(header)

        form = QWidget()
        form.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        f_layout = QVBoxLayout(form)
        f_layout.setContentsMargins(36, 32, 36, 32)
        f_layout.setAlignment(Qt.AlignmentFlag.AlignTop)

        cfg = self.app.getConfig()

        f_layout.addWidget(self.lbl("Razón Social / Nombre de la Tienda"))
        self.txtNombre = self.campo(cfg.getNombre())
        f_layout.addWidget(self.txtNombre)
        f_layout.addSpacing(16)

        f_layout.addWidget(self.lbl("Sucursal"))
        self.txtSucursal = self.campo(cfg.getSucursal())
        f_layout.addWidget(self.txtSucursal)
        f_layout.addSpacing(16)

        f_layout.addWidget(self.lbl("RFC"))
        self.txtRFC = self.campo(cfg.getRfc())
        f_layout.addWidget(self.txtRFC)
        f_layout.addSpacing(32)

        btnGuardar = Estilos.botonPrimario("✓  ACTUALIZAR CONFIGURACIÓN")
        btnGuardar.setFont(QFont("SansSerif", 16, QFont.Weight.Bold))
        btnGuardar.setFixedHeight(60)
        btnGuardar.clicked.connect(self.guardar)
        f_layout.addWidget(btnGuardar)

        f_layout.addSpacing(20)

        btnReiniciar = Estilos.botonPeligro("REINICIAR SISTEMA")
        btnReiniciar.setFont(QFont("SansSerif", 16, QFont.Weight.Bold))
        btnReiniciar.setFixedHeight(60)
        btnReiniciar.clicked.connect(self.reiniciarSistema)
        f_layout.addWidget(btnReiniciar)

        scroll = QScrollArea()
        scroll.setWidgetResizable(True)
        scroll.setWidget(form)
        scroll.setStyleSheet("border: none;")
        
        layout.addWidget(scroll)

    def guardar(self):
        nuevoNombre = self.txtNombre.text().strip()
        nuevaSucursal = self.txtSucursal.text().strip()
        nuevoRFC = self.txtRFC.text().strip()

        if not nuevoNombre or not nuevaSucursal or not nuevoRFC:
            QMessageBox.information(self.window(), "Aviso", "por favor llena todos los campos")
            return

        nuevaConfig = ConfiguracionTienda(nuevoNombre, nuevaSucursal, nuevoRFC)
        self.app.guardarConfiguracion(nuevaConfig)
        self.app.getVentanaPrincipal().actualizarTitulo()

        QMessageBox.information(self.window(), "Exito", "configuracion guardada exitosamente en la base de datos")

    def reiniciarSistema(self):
        res1 = QMessageBox.warning(self.window(), "ADVERTENCIA 1 DE 3", "¿estas seguro? se borraran todas las ventas y movimientos del sistema", QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
        if res1 == QMessageBox.StandardButton.Yes:
            res2 = QMessageBox.critical(self.window(), "ADVERTENCIA 2 DE 3", "esta accion no se puede deshacer; perderas todo el historial financiero, ¿continuar?", QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
            if res2 == QMessageBox.StandardButton.Yes:
                text, ok = QInputDialog.getText(self.window(), "Confirmacion", "para confirmar escribe la palabra: ELIMINAR TODO")
                if ok and text.strip().upper() == "ELIMINAR TODO":
                    self.app.reiniciarSistemaCompleto()
                else:
                    QMessageBox.information(self.window(), "Cancelado", "reinicio cancelado, la palabra no coincide")

    def lbl(self, text):
        l = QLabel(text.upper())
        l.setFont(Estilos.FUENTE_XS)
        l.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        return l

    def campo(self, valor):
        tf = QLineEdit(valor)
        tf.setFont(QFont("SansSerif", 20, QFont.Weight.Bold))
        tf.setFixedHeight(56)
        tf.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 12px 16px;
            }}
        """)
        return tf
