from PyQt6.QtWidgets import QMainWindow, QWidget, QVBoxLayout, QHBoxLayout, QStackedWidget, QLabel, QPushButton, QFrame, QSizePolicy
from PyQt6.QtCore import Qt, QSize
from PyQt6.QtGui import QFont, QColor
from Vista.Estilos import Estilos

class VentanaPrincipal:
    TAB_VENTA = 0
    TAB_INVENTARIO = 1
    TAB_CAJA = 2
    TAB_PROVEEDORES = 3
    TAB_USUARIOS = 4
    TAB_REPORTES = 5
    TAB_CONFIG = 6

    def __init__(self, app, ventaCtrl, invCtrl, cajaCtrl, provCtrl, usuCtrl):
        self.app = app
        self.ventaCtrl = ventaCtrl
        self.invCtrl = invCtrl
        self.cajaCtrl = cajaCtrl
        self.provCtrl = provCtrl
        self.usuCtrl = usuCtrl
        
        self.frame = QMainWindow()
        self.construir()

    def construir(self):
        self.frame.setWindowTitle("")
        self.frame.setMinimumSize(1280, 800)

        root = QWidget()
        root.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")
        layout_root = QHBoxLayout(root)
        layout_root.setContentsMargins(0, 0, 0, 0)
        layout_root.setSpacing(0)

        sidebar = self.construirSidebar()
        layout_root.addWidget(sidebar)

        self.cardLayout = QStackedWidget()
        self.cardLayout.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")
        
        from Vista.VentaPanel import VentaPanel
        from Vista.InventarioPanel import InventarioPanel
        from Vista.CajaPanel import CajaPanel
        from Vista.ProveedorPanel import ProveedorPanel
        from Vista.UsuariosPanel import UsuariosPanel
        from Vista.ReportesPanel import ReportesPanel
        from Vista.ConfigPanel import ConfigPanel

        self.ventaPanel = VentaPanel(self.ventaCtrl, self.app.getConfig(), self.app.getUsuarioActivo())
        self.inventarioPanel = InventarioPanel(self.invCtrl, self.app.getUsuarioActivo())
        self.cajaPanel = CajaPanel(self.cajaCtrl, self.app.getUsuarioActivo(), self)
        self.proveedorPanel = ProveedorPanel(self.provCtrl, self.app.getUsuarioActivo())
        self.usuariosPanel = UsuariosPanel(self.usuCtrl)
        self.reportesPanel = ReportesPanel(self.app)
        self.configPanel = ConfigPanel(self.app)

        self.ventaCtrl.setPanel(self.ventaPanel)
        self.invCtrl.setPanel(self.inventarioPanel)
        self.cajaCtrl.setPanel(self.cajaPanel)
        self.provCtrl.setPanel(self.proveedorPanel)
        self.usuCtrl.setPanel(self.usuariosPanel)

        self.cardLayout.addWidget(self.ventaPanel)
        self.cardLayout.addWidget(self.inventarioPanel)
        self.cardLayout.addWidget(self.cajaPanel)
        self.cardLayout.addWidget(self.proveedorPanel)
        self.cardLayout.addWidget(self.usuariosPanel)
        self.cardLayout.addWidget(self.reportesPanel)
        self.cardLayout.addWidget(self.configPanel)

        layout_root.addWidget(self.cardLayout)
        
        self.frame.setCentralWidget(root)
        self.actualizarTitulo()

    def construirSidebar(self):
        sidebar = QWidget()
        sidebar.setFixedWidth(220)
        sidebar.setStyleSheet(f"background-color: {Estilos.BG_OSCURO.name()};")
        layout = QVBoxLayout(sidebar)
        layout.setContentsMargins(0, 0, 0, 0)
        layout.setSpacing(0)

        logo = QWidget()
        logo.setMinimumHeight(80)
        logo_layout = QHBoxLayout(logo)
        logo_layout.setContentsMargins(16, 20, 16, 20)

        self.labelNombreTienda = QLabel(f" {self.app.getConfig().getNombre()}")
        self.labelNombreTienda.setFont(Estilos.FUENTE_BOLD)
        self.labelNombreTienda.setStyleSheet("color: white; border: none;")
        logo_layout.addWidget(self.labelNombreTienda)
        
        layout.addWidget(logo)

        sep = QFrame()
        sep.setFrameShape(QFrame.Shape.HLine)
        sep.setStyleSheet("color: #3F3F46; background-color: #3F3F46;")
        sep.setFixedHeight(1)
        layout.addWidget(sep)
        
        layout.addSpacing(12)

        layout.addWidget(self.itemMenu("Venta", self.TAB_VENTA))
        layout.addWidget(self.itemMenu("Inventario", self.TAB_INVENTARIO))
        layout.addWidget(self.itemMenu("Proveedores", self.TAB_PROVEEDORES))
        layout.addWidget(self.itemMenu("Flujo de Caja", self.TAB_CAJA))
        layout.addWidget(self.itemMenu("Reportes", self.TAB_REPORTES))

        if self.app.getUsuarioActivo().esAdmin():
            layout.addSpacing(16)
            secLabel = QLabel("  CONFIGURACIÓN")
            secLabel.setFont(Estilos.FUENTE_XS)
            secLabel.setStyleSheet("color: #3F3F46; border: none;")
            layout.addWidget(secLabel)
            layout.addWidget(self.itemMenu("Personal", self.TAB_USUARIOS))
            layout.addWidget(self.itemMenu("Sistema", self.TAB_CONFIG))

        layout.addStretch()

        footer = QWidget()
        footer.setStyleSheet(f"background-color: {Estilos.BG_MUY_OSCURO.name()};")
        footer_layout = QVBoxLayout(footer)
        footer_layout.setContentsMargins(14, 14, 14, 14)
        
        u = self.app.getUsuarioActivo()
        nombreLbl = QLabel(f"{u.getNombre()[0]}  {u.getNombre()}")
        nombreLbl.setFont(Estilos.FUENTE_BOLD)
        nombreLbl.setStyleSheet("color: white; border: none;")
        
        rolLbl = QLabel(u.getRol())
        rolLbl.setFont(Estilos.FUENTE_XS)
        rolLbl.setStyleSheet(f"color: {Estilos.INDIGO.name()}; border: none;")
        
        btnSalir = QPushButton("Cerrar Sesión")
        btnSalir.setFont(Estilos.FUENTE_XS)
        btnSalir.setCursor(Qt.CursorShape.PointingHandCursor)
        btnSalir.setStyleSheet(f"""
            QPushButton {{
                color: {Estilos.ROSE.name()};
                background-color: transparent;
                border: none;
                text-align: left;
            }}
            QPushButton:hover {{
                color: {Estilos.ROSE_OSCURO.name()};
            }}
        """)
        btnSalir.clicked.connect(self.app.onCerrarSesion)

        footer_layout.addWidget(nombreLbl)
        footer_layout.addSpacing(4)
        footer_layout.addWidget(rolLbl)
        footer_layout.addSpacing(10)
        footer_layout.addWidget(btnSalir)

        layout.addWidget(footer)
        return sidebar

    def itemMenu(self, label, tabId):
        btn = QPushButton(f"  {label}")
        btn.setFont(Estilos.FUENTE_XS)
        btn.setCursor(Qt.CursorShape.PointingHandCursor)
        btn.setStyleSheet(f"""
            QPushButton {{
                color: #A1A1AA;
                background-color: transparent;
                border: none;
                text-align: left;
                padding: 12px 20px;
            }}
            QPushButton:hover {{
                background-color: #27272A;
                color: white;
            }}
        """)
        btn.clicked.connect(lambda checked, idx=tabId: self.cambiarTab(idx))
        return btn

    def cambiarTab(self, tab):
        self.cardLayout.setCurrentIndex(tab)
        if tab == self.TAB_INVENTARIO:
            self.inventarioPanel.refrescar()
        elif tab == self.TAB_CAJA:
            self.cajaPanel.refrescar()
        elif tab == self.TAB_PROVEEDORES:
            self.proveedorPanel.refrescarProveedores()
            self.proveedorPanel.refrescarOrdenes()
            self.proveedorPanel.refrescarCuentas()
        elif tab == self.TAB_USUARIOS:
            self.usuariosPanel.refrescar()
        elif tab == self.TAB_REPORTES:
            self.reportesPanel.refrescar()

    def actualizarTitulo(self):
        config = self.app.getConfig()
        if hasattr(self, 'labelNombreTienda') and self.labelNombreTienda:
            self.labelNombreTienda.setText(f"🏪 {config.getNombre()}")
        if self.frame:
            self.frame.setWindowTitle(f"{config.getNombre()} - Sistema de Punto de Venta")

    def refrescarInventario(self):
        self.inventarioPanel.refrescar()

    def refrescarCaja(self):
        if self.cajaCtrl.isCajaAbierta():
            self.cajaPanel.refrescar()

    def mostrar(self):
        self.frame.show()

    def ocultar(self):
        self.frame.hide()

    def getFrame(self):
        return self.frame
