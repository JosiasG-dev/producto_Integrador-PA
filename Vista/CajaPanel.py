from PyQt6.QtWidgets import QWidget, QVBoxLayout, QHBoxLayout, QLabel, QStackedWidget, QPushButton, QLineEdit, QTableWidget, QTableWidgetItem, QHeaderView, QFrame
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont, QColor
from Vista.Estilos import Estilos
from Modelo.Movimiento import Movimiento, Tipo as TipoMovimiento
from datetime import datetime

class CajaPanel(QWidget):
    TAB_CERRADA = 0
    TAB_ABIERTA = 1

    def __init__(self, ctrl, usuario, ventana):
        super().__init__()
        self.ctrl = ctrl
        self.usuario = usuario
        self.ventana = ventana
        self.construir()

    def construir(self):
        self.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")
        layout = QVBoxLayout(self)
        layout.setContentsMargins(24, 24, 24, 24)

        self.cardLayout = QStackedWidget()
        self.cardLayout.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")

        self.cardLayout.addWidget(self.construirPanelCerrada())
        self.cardLayout.addWidget(self.construirPanelAbierta())

        layout.addWidget(self.cardLayout)
        self.refrescar()

    def construirPanelCerrada(self):
        p = QWidget()
        p_layout = QVBoxLayout(p)
        p_layout.setAlignment(Qt.AlignmentFlag.AlignCenter)

        tarjeta = QWidget()
        tarjeta.setFixedSize(420, 440)
        tarjeta.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()}; border-radius: 8px;")
        t_layout = QVBoxLayout(tarjeta)
        t_layout.setContentsMargins(44, 40, 44, 40)
        t_layout.setAlignment(Qt.AlignmentFlag.AlignTop)

        icono = QLabel("💵")
        icono.setFont(QFont("SansSerif", 64))
        icono.setAlignment(Qt.AlignmentFlag.AlignCenter)
        icono.setStyleSheet("border: none;")
        t_layout.addWidget(icono)

        t_layout.addSpacing(16)

        tit = QLabel("Terminal Cerrada")
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setAlignment(Qt.AlignmentFlag.AlignCenter)
        tit.setStyleSheet("border: none;")
        t_layout.addWidget(tit)

        resp = QLabel(f"Responsable: {self.usuario.getNombre()}")
        resp.setFont(Estilos.FUENTE_SMALL)
        resp.setStyleSheet(f"color: {Estilos.TEXTO_SECUNDARIO.name()}; border: none;")
        resp.setAlignment(Qt.AlignmentFlag.AlignCenter)
        t_layout.addWidget(resp)

        t_layout.addSpacing(28)

        lblFondoTexto = QLabel("FONDO INICIAL ($)")
        lblFondoTexto.setFont(Estilos.FUENTE_XS)
        lblFondoTexto.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        t_layout.addWidget(lblFondoTexto)

        self.txtFondo = QLineEdit("0.00")
        self.txtFondo.setFont(QFont("SansSerif", 26, QFont.Weight.Bold))
        self.txtFondo.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 12px 16px;
                border-radius: 4px;
            }}
        """)
        t_layout.addWidget(self.txtFondo)

        t_layout.addSpacing(24)

        btnAbrir = Estilos.botonPrimario("ABRIR TERMINAL")
        btnAbrir.setFont(QFont("SansSerif", 16, QFont.Weight.Bold))
        btnAbrir.setFixedHeight(56)
        btnAbrir.clicked.connect(self.abrirCaja)
        t_layout.addWidget(btnAbrir)

        p_layout.addWidget(tarjeta)
        return p

    def abrirCaja(self):
        try:
            fondo = float(self.txtFondo.text().strip())
            self.ctrl.abrirCaja(fondo)
        except ValueError:
            self.ctrl.abrirCaja(0)

    def construirPanelAbierta(self):
        p = QWidget()
        layout = QVBoxLayout(p)
        layout.setSpacing(16)
        layout.setContentsMargins(0, 0, 0, 0)

        header = QWidget()
        header.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        h_layout = QHBoxLayout(header)
        h_layout.setContentsMargins(24, 20, 24, 20)

        tit = QLabel("GESTIÓN DE CAJA  —  Operativa")
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setStyleSheet("border: none;")

        btnRetiro = Estilos.botonPeligro("↑ Retiro / Pago")
        btnRetiro.setFixedSize(160, 42)
        btnRetiro.clicked.connect(self.abrirDialogoRetiro)

        h_layout.addWidget(tit)
        h_layout.addStretch()
        h_layout.addWidget(btnRetiro)

        layout.addWidget(header)

        tarjetas = QWidget()
        tarjetas.setFixedHeight(110)
        t_layout = QHBoxLayout(tarjetas)
        t_layout.setSpacing(16)
        t_layout.setContentsMargins(0, 0, 0, 0)

        cardFondo, self.lblEfectivo = self.tarjetaCaja("Total Ventas", "$0.00", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL)
        cardEfectivo, self.lblEfectivoTotal = self.tarjetaCaja("Efectivo en Caja", "$0.00", Estilos.INDIGO, QColor("white"))

        t_layout.addWidget(cardFondo)
        t_layout.addWidget(cardEfectivo)

        layout.addWidget(tarjetas)

        self.tablaMovs = QTableWidget(0, 4)
        self.tablaMovs.setHorizontalHeaderLabels(["Hora", "Tipo", "Concepto", "Monto"])
        self.tablaMovs.setFont(Estilos.FUENTE_NORMAL)
        self.tablaMovs.horizontalHeader().setFont(Estilos.FUENTE_XS)
        self.tablaMovs.horizontalHeader().setStyleSheet(f"background-color: {Estilos.BG_ZINC_100.name()};")
        self.tablaMovs.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        self.tablaMovs.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        self.tablaMovs.horizontalHeader().setSectionResizeMode(2, QHeaderView.ResizeMode.Stretch)
        self.tablaMovs.setColumnWidth(0, 100)
        self.tablaMovs.setColumnWidth(1, 110)
        self.tablaMovs.setColumnWidth(3, 120)

        layout.addWidget(self.tablaMovs)

        return p

    def tarjetaCaja(self, label, valor, bg, fg):
        card = QWidget()
        card.setStyleSheet(f"background-color: {bg.name()}; border: 1px solid {Estilos.BORDE.name() if bg == Estilos.BG_BLANCO else bg.name()}; border-radius: 4px;")
        layout = QVBoxLayout(card)
        layout.setContentsMargins(22, 18, 22, 18)
        
        lLbl = QLabel(label.upper())
        lLbl.setFont(Estilos.FUENTE_XS)
        lLbl.setStyleSheet(f"color: {'#C7D2FE' if bg == Estilos.INDIGO else Estilos.TEXTO_TENUE.name()}; border: none;")
        
        lVal = QLabel(valor)
        lVal.setFont(QFont("SansSerif", 34, QFont.Weight.Bold))
        lVal.setStyleSheet(f"color: {fg.name()}; border: none;")
        
        layout.addWidget(lLbl)
        layout.addWidget(lVal)
        
        return card, lVal

    def abrirDialogoRetiro(self):
        from Vista.RetiroDialog import RetiroDialog
        dlg = RetiroDialog(self.ventana.getFrame(), self.ctrl)
        dlg.exec()
        self.refrescar()

    def refrescar(self):
        if self.ctrl.isCajaAbierta():
            self.cardLayout.setCurrentIndex(self.TAB_ABIERTA)
            totalGeneral = sum(m.getMonto() for m in self.ctrl.getMovimientos() if m.getTipo() == TipoMovimiento.VENTA)
            
            if hasattr(self, 'lblEfectivo'):
                self.lblEfectivo.setText(f"${totalGeneral:.2f}")
            if hasattr(self, 'lblEfectivoTotal'):
                self.lblEfectivoTotal.setText(f"${self.ctrl.getEfectivoEsperado():.2f}")
                
            self.actualizarTablaMovimientos()
        else:
            self.cardLayout.setCurrentIndex(self.TAB_CERRADA)

    def actualizarTablaMovimientos(self):
        movs = list(reversed(self.ctrl.getMovimientos()))
        self.tablaMovs.setRowCount(len(movs))
        for i, m in enumerate(movs):
            fecha = m.getFecha().strftime("%H:%M:%S") if m.getFecha() else ""
            self.tablaMovs.setItem(i, 0, QTableWidgetItem(fecha))
            self.tablaMovs.setItem(i, 1, QTableWidgetItem(m.getTipo().name))
            self.tablaMovs.setItem(i, 2, QTableWidgetItem(m.getDescripcion()))
            monto_str = f"+${m.getMonto():.2f}" if m.esIngreso() else f"-${m.getMonto():.2f}"
            self.tablaMovs.setItem(i, 3, QTableWidgetItem(monto_str))
