from PyQt6.QtWidgets import QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton, QComboBox, QMessageBox, QFileDialog, QWidget
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont
from Vista.Estilos import Estilos
from Modelo.Producto import Producto
from Modelo.DatosIniciales import DatosIniciales
from Util.RutaBase import RutaBase
import os
import shutil

class ProductoDialog(QDialog):
    def __init__(self, parent, p, ctrl):
        super().__init__(parent)
        self.ctrl = ctrl
        self.producto = p
        self.rutaImagen = ""
        self.setWindowTitle("Nuevo Producto" if p is None else "Editar Producto")
        self.setModal(True)
        self.construir()
        if p:
            self.cargar(p)

    def construir(self):
        self.setFixedSize(480, 580)
        panel = QWidget(self)
        panel.resize(480, 580)
        panel.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")

        tit = QLabel("NUEVO PRODUCTO" if self.producto is None else "EDITAR PRODUCTO", panel)
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setGeometry(28, 18, 420, 30)

        x, aw, fh = 28, 424, 36

        self.lbl("SKU / Codigo", x, 60, panel)
        self.txtId = self.campo(x, 76, aw, fh, panel)
        self.txtId.setText(self.ctrl.generarNuevoId() if self.producto is None else "")
        self.txtId.setEnabled(self.producto is None)

        self.lbl("Nombre del Producto", x, 124, panel)
        self.txtNombre = self.campo(x, 140, aw, fh, panel)

        self.lbl("Precio Publico ($)", x, 188, panel)
        self.txtPrecio = self.campo(x, 204, 200, fh, panel)
        self.txtPrecio.setText("0.00")

        self.lbl("Existencia Actual", x + 210, 188, panel)
        self.txtStock = self.campo(x + 210, 204, 214, fh, panel)
        self.txtStock.setText("0")

        self.lbl("Stock Minimo (limite de alerta)", x, 252, panel)
        self.txtStockMin = self.campo(x, 268, 200, fh, panel)
        self.txtStockMin.setText("5")

        self.lbl("Categoria", x, 316, panel)
        self.cmbCategoria = QComboBox(panel)
        self.cmbCategoria.addItems(DatosIniciales.getCategorias())
        self.cmbCategoria.setFont(Estilos.FUENTE_BOLD)
        self.cmbCategoria.setGeometry(x, 332, 200, fh)

        self.lbl("Unidad de Medida", x + 210, 316, panel)
        self.cmbUnidad = QComboBox(panel)
        self.cmbUnidad.addItems(DatosIniciales.getUnidades())
        self.cmbUnidad.setFont(Estilos.FUENTE_BOLD)
        self.cmbUnidad.setGeometry(x + 210, 332, 214, fh)

        self.lbl("Imagen del Producto", x, 380, panel)
        self.btnImagen = QPushButton("Seleccionar Imagen", panel)
        self.btnImagen.setFont(Estilos.FUENTE_XS)
        self.btnImagen.setGeometry(x, 396, 200, 30)
        self.btnImagen.clicked.connect(self.seleccionarImagen)

        if self.producto:
            btnEliminar = Estilos.botonPeligro("Eliminar")
            btnEliminar.setParent(panel)
            btnEliminar.setGeometry(28, 480, 100, 38)
            btnEliminar.clicked.connect(self.eliminar)

        btnCancelar = Estilos.botonSecundario("Cancelar")
        btnCancelar.setParent(panel)
        btnCancelar.setGeometry(240, 480, 100, 38)
        btnCancelar.clicked.connect(self.reject)

        btnGuardar = Estilos.botonPrimario("Guardar")
        btnGuardar.setParent(panel)
        btnGuardar.setGeometry(350, 480, 102, 38)
        btnGuardar.clicked.connect(self.guardar)

    def seleccionarImagen(self):
        archivo, _ = QFileDialog.getOpenFileName(self, "Seleccionar Imagen", "", "Imagenes (*.png *.jpg *.jpeg)")
        if archivo:
            carpetaDestino = RutaBase.getImages()
            nombre = os.path.basename(archivo)
            archivoDestino = os.path.join(carpetaDestino, nombre)
            try:
                shutil.copy2(archivo, archivoDestino)
                self.rutaImagen = "Images/" + nombre
                self.btnImagen.setText("✅ " + nombre)
            except Exception as ex:
                QMessageBox.warning(self, "Error", f"Error al copiar la imagen: {ex}")

    def eliminar(self):
        ok = QMessageBox.question(self, "Confirmar", f"Eliminar producto {self.producto.getNombre()}?", QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
        if ok == QMessageBox.StandardButton.Yes:
            self.ctrl.eliminarProducto(self.producto.getId())
            self.accept()

    def cargar(self, p):
        self.txtId.setText(p.getId())
        self.txtNombre.setText(p.getNombre())
        self.txtPrecio.setText(f"{p.getPrecio():.2f}")
        self.txtStock.setText(f"{p.getStock():.1f}")
        self.txtStockMin.setText(str(int(p.getStockMinimo())))
        self.cmbCategoria.setCurrentText(p.getCategoria())
        self.cmbUnidad.setCurrentText(p.getUnidad())
        self.rutaImagen = p.getImagenRuta()

    def guardar(self):
        try:
            id_prod = self.txtId.text().strip()
            nom = self.txtNombre.text().strip()
            precio = float(self.txtPrecio.text().strip())
            stock = float(self.txtStock.text().strip())
            stockMin = float(self.txtStockMin.text().strip())
            cat = self.cmbCategoria.currentText()
            unidad = self.cmbUnidad.currentText()

            if not id_prod or not nom:
                QMessageBox.critical(self, "Error", "SKU y Nombre son obligatorios")
                return

            p = Producto(id_prod, nom, precio, stock, cat, unidad, self.rutaImagen)
            p.setStockMinimo(stockMin)
            self.ctrl.guardarProducto(p)
            self.accept()
        except ValueError:
            QMessageBox.critical(self, "Error", "Datos numéricos inválidos")

    def lbl(self, texto, x, y, parent):
        l = QLabel(texto.upper(), parent)
        l.setFont(Estilos.FUENTE_XS)
        l.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        l.setGeometry(x, y, 300, 14)
        return l

    def campo(self, x, y, w, h, parent):
        tf = QLineEdit(parent)
        tf.setFont(Estilos.FUENTE_BOLD)
        tf.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 6px 10px;
            }}
        """)
        tf.setGeometry(x, y, w, h)
        return tf
