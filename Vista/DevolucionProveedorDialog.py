from PyQt6.QtWidgets import QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton, QMessageBox, QWidget
from PyQt6.QtCore import Qt
from Vista.Estilos import Estilos

class DevolucionProveedorDialog(QDialog):
    def __init__(self, parent, p, ctrl):
        super().__init__(parent)
        self.ctrl = ctrl
        self.producto = p
        self.setWindowTitle("Devolucion a Proveedor")
        self.setModal(True)
        self.construir()

    def construir(self):
        self.setFixedSize(380, 300)
        panel = QWidget(self)
        panel.resize(380, 300)
        panel.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")

        tit = QLabel("DEVOLVER PRODUCTOS", panel)
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setGeometry(24, 18, 300, 30)

        lblProd = QLabel(f"Producto: {self.producto.getNombre()}", panel)
        lblProd.setFont(Estilos.FUENTE_BOLD)
        lblProd.setGeometry(24, 60, 330, 20)

        lblStock = QLabel(f"Stock actual: {self.producto.getStock()} {self.producto.getUnidad()}", panel)
        lblStock.setFont(Estilos.FUENTE_NORMAL)
        lblStock.setStyleSheet(f"color: {Estilos.TEXTO_SECUNDARIO.name()};")
        lblStock.setGeometry(24, 85, 330, 20)

        lblCant = QLabel("CANTIDAD A DEVOLVER:", panel)
        lblCant.setFont(Estilos.FUENTE_XS)
        lblCant.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        lblCant.setGeometry(24, 130, 300, 14)

        self.txtCantidad = QLineEdit(panel)
        self.txtCantidad.setFont(Estilos.FUENTE_BOLD)
        self.txtCantidad.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 6px 10px;
            }}
        """)
        self.txtCantidad.setGeometry(24, 148, 315, 36)

        btnCancelar = Estilos.botonSecundario("Cancelar")
        btnCancelar.setParent(panel)
        btnCancelar.setGeometry(110, 210, 110, 38)
        btnCancelar.clicked.connect(self.reject)

        btnGuardar = Estilos.botonPeligro("Devolver")
        btnGuardar.setParent(panel)
        btnGuardar.setGeometry(230, 210, 110, 38)
        btnGuardar.clicked.connect(self.guardar)

    def guardar(self):
        try:
            cantidad = float(self.txtCantidad.text().strip())
            if cantidad <= 0:
                QMessageBox.warning(self, "Aviso", "Ingrese una cantidad valida mayor a 0")
                return
            if cantidad > self.producto.getStock():
                QMessageBox.warning(self, "Aviso", "No puede devolver mas del stock existente")
                return
                
            self.producto.setStock(self.producto.getStock() - cantidad)
            self.ctrl.guardarProducto(self.producto)
            QMessageBox.information(self, "Exito", "Devolucion procesada exitosamente")
            self.accept()
        except ValueError:
            QMessageBox.critical(self, "Error", "Numero invalido")
