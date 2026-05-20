from PyQt6.QtWidgets import QDialog, QVBoxLayout, QLabel, QFrame, QPushButton, QScrollArea, QWidget
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont, QColor
from PyQt6.QtPrintSupport import QPrinter, QPrintDialog
from PyQt6.QtGui import QPainter
from Vista.Estilos import Estilos

class TicketDialog(QDialog):
    def __init__(self, parent, venta, config, cambio, descuento=None, efectivoRecibido=None):
        super().__init__(parent)
        self.setWindowTitle("Ticket de Venta")
        self.setModal(True)
        self.venta = venta
        self.config = config
        self.cambio = cambio
        
        if efectivoRecibido is None:
            self.efectivoRecibido = descuento
            self.descuento = venta.getDescuento() if venta.getDescuento() > 0 else cambio
        else:
            self.efectivoRecibido = efectivoRecibido
            self.descuento = venta.getDescuento() if venta.getDescuento() > 0 else descuento
            
        self.construir()

    def construir(self):
        self.setFixedSize(430, 700)
        
        contenedor = QWidget()
        contenedor.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")
        
        self.panelTicket = QWidget(contenedor)
        self.panelTicket.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")
        
        y = 0
        
        barra = QFrame(self.panelTicket)
        barra.setStyleSheet(f"background-color: {Estilos.INDIGO.name()};")
        barra.setGeometry(0, 0, 380, 8)
        y = 18
        
        lblNombre = self.centrado(self.config.getNombre(), QFont("Monospaced", 15, QFont.Weight.Bold), y)
        self.panelTicket.children().append(lblNombre)
        y += 22
        
        lblSuc = self.centrado(self.config.getSucursal(), Estilos.FUENTE_XS, y)
        lblSuc.setStyleSheet(f"color: {Estilos.TEXTO_SECUNDARIO.name()};")
        y += 16
        
        lblRfc = self.centrado(f"RFC: {self.config.getRfc()}", Estilos.FUENTE_XS, y)
        lblRfc.setStyleSheet(f"color: {Estilos.TEXTO_SECUNDARIO.name()};")
        y += 16
        
        self.sep(y)
        y += 14
        
        fecha_str = self.venta.getFecha().strftime("%d/%m/%Y %H:%M:%S") if self.venta.getFecha() else ""
        self.centrado(fecha_str, Estilos.FUENTE_XS, y)
        y += 16
        
        self.centrado(f"Cajero: {self.venta.getCajero().upper()}", Estilos.FUENTE_XS, y)
        y += 16
        
        lblNum = self.centrado(f"Ticket #{self.venta.getId()}", Estilos.FUENTE_XS, y)
        lblNum.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()};")
        y += 16
        
        self.sep(y)
        y += 14
        
        for item in self.venta.getItems():
            cod = QLabel(f"[{item.getProducto().getId()}] {item.getProducto().getNombre().upper()}", self.panelTicket)
            cod.setFont(Estilos.FUENTE_XS)
            cod.setGeometry(14, y, 240, 16)
            
            sub = QLabel(f"${item.getSubtotal():.2f}", self.panelTicket)
            sub.setFont(Estilos.FUENTE_XS)
            sub.setAlignment(Qt.AlignmentFlag.AlignRight)
            sub.setGeometry(256, y, 110, 16)
            y += 16
            
            det = QLabel(f"  {item.getCantidad():.2f} x ${item.getProducto().getPrecio():.2f}", self.panelTicket)
            det.setFont(Estilos.FUENTE_XS)
            det.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()};")
            det.setGeometry(14, y, 240, 14)
            y += 18
            
        self.sep(y)
        y += 10
        
        precioTotal = sum(item.getSubtotal() for item in self.venta.getItems())
        montoAPagar = self.venta.getTotal()
        
        self.fila("Precio total de compra:", f"${precioTotal:.2f}", y, Estilos.TEXTO_SECUNDARIO)
        y += 18
        
        colorDesc = Estilos.EMERALD if self.descuento > 0 else Estilos.TEXTO_TENUE
        self.fila("Monto de descuento:", f"-${self.descuento:.2f}", y, colorDesc)
        y += 18
        
        self.sep(y)
        y += 10
        
        self.filaGrande("MONTO A PAGAR:", f"${montoAPagar:.2f}", y, Estilos.TEXTO_PRINCIPAL)
        y += 22
        
        self.sep(y)
        y += 10
        
        self.fila("Efectivo recibido:", f"${self.efectivoRecibido:.2f}", y, Estilos.TEXTO_SECUNDARIO)
        y += 18
        
        self.fila("Cambio:", f"${self.cambio:.2f}", y, Estilos.EMERALD)
        y += 20
        
        gracias = self.centrado("Gracias por su compra", Estilos.FUENTE_SMALL, y)
        gracias.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()};")
        y += 30
        
        self.panelTicket.setGeometry(0, 0, 380, y)
        
        btnImprimir = QPushButton("Imprimir / Cerrar", contenedor)
        btnImprimir.setFont(Estilos.FUENTE_BOLD)
        btnImprimir.setCursor(Qt.CursorShape.PointingHandCursor)
        btnImprimir.setStyleSheet(f"""
            QPushButton {{
                background-color: {Estilos.BG_OSCURO.name()};
                color: white;
                border: none;
            }}
        """)
        btnImprimir.setGeometry(14, y + 4, 352, 44)
        btnImprimir.clicked.connect(self.imprimir)
        
        contenedor.setMinimumSize(380, y + 56)
        
        scroll = QScrollArea(self)
        scroll.setWidget(contenedor)
        scroll.setStyleSheet("border: none;")
        
        layout = QVBoxLayout(self)
        layout.setContentsMargins(0, 0, 0, 0)
        layout.addWidget(scroll)

    def centrado(self, texto, fuente, y):
        lbl = QLabel(texto, self.panelTicket)
        lbl.setFont(fuente)
        lbl.setAlignment(Qt.AlignmentFlag.AlignCenter)
        lbl.setGeometry(0, y, 380, 18)
        return lbl

    def sep(self, y):
        s = QFrame(self.panelTicket)
        s.setFrameShape(QFrame.Shape.HLine)
        s.setStyleSheet(f"color: {Estilos.BORDE.name()}; background-color: {Estilos.BORDE.name()};")
        s.setGeometry(14, y, 352, 1)
        return s

    def fila(self, label, valor, y, colorValor):
        lbl = QLabel(label, self.panelTicket)
        lbl.setFont(Estilos.FUENTE_XS)
        lbl.setStyleSheet(f"color: {Estilos.TEXTO_SECUNDARIO.name()};")
        lbl.setGeometry(14, y, 180, 16)
        
        val = QLabel(valor, self.panelTicket)
        val.setFont(Estilos.FUENTE_XS)
        val.setStyleSheet(f"color: {colorValor.name()};")
        val.setAlignment(Qt.AlignmentFlag.AlignRight)
        val.setGeometry(200, y, 166, 16)

    def filaGrande(self, label, valor, y, colorValor):
        lbl = QLabel(label, self.panelTicket)
        lbl.setFont(Estilos.FUENTE_BOLD)
        lbl.setStyleSheet(f"color: {Estilos.TEXTO_PRINCIPAL.name()};")
        lbl.setGeometry(14, y, 180, 20)
        
        val = QLabel(valor, self.panelTicket)
        val.setFont(QFont("Monospaced", 15, QFont.Weight.Bold))
        val.setStyleSheet(f"color: {colorValor.name()};")
        val.setAlignment(Qt.AlignmentFlag.AlignRight)
        val.setGeometry(180, y, 186, 20)

    def imprimir(self):
        printer = QPrinter(QPrinter.PrinterMode.HighResolution)
        dialog = QPrintDialog(printer, self)
        if dialog.exec() == QPrintDialog.DialogCode.Accepted:
            painter = QPainter()
            painter.begin(printer)
            self.panelTicket.render(painter)
            painter.end()
        self.accept()

    def mostrar(self):
        self.exec()
