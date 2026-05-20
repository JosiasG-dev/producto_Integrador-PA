from PyQt6.QtWidgets import QWidget, QVBoxLayout, QHBoxLayout, QLabel, QPushButton, QTableWidget, QTableWidgetItem, QHeaderView
from PyQt6.QtCore import Qt
from Vista.Estilos import Estilos

class UsuariosPanel(QWidget):
    def __init__(self, ctrl):
        super().__init__()
        self.ctrl = ctrl
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
        
        tit = QLabel("RRHH  —  Equipo de Operación")
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setStyleSheet("border: none;")
        
        btnNuevo = Estilos.botonPrimario("+ Alta Personal")
        btnNuevo.setFixedSize(150, 42)
        btnNuevo.clicked.connect(lambda: self.abrirFormulario(None))
        
        h_layout.addWidget(tit)
        h_layout.addStretch()
        h_layout.addWidget(btnNuevo)
        
        layout.addWidget(header)

        self.tabla = QTableWidget(0, 6)
        self.tabla.setHorizontalHeaderLabels(["ID", "Nombre", "Rol", "Usuario Red", "Edad", "Sexo"])
        self.tabla.setFont(Estilos.FUENTE_NORMAL)
        self.tabla.horizontalHeader().setFont(Estilos.FUENTE_XS)
        self.tabla.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        self.tabla.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        self.tabla.setSelectionBehavior(QTableWidget.SelectionBehavior.SelectRows)
        self.tabla.horizontalHeader().setSectionResizeMode(1, QHeaderView.ResizeMode.Stretch)
        self.tabla.horizontalHeader().setSectionResizeMode(3, QHeaderView.ResizeMode.Stretch)
        self.tabla.setColumnWidth(0, 60)
        self.tabla.setColumnWidth(2, 160)
        self.tabla.setColumnWidth(4, 70)
        self.tabla.setColumnWidth(5, 100)
        
        self.tabla.itemDoubleClicked.connect(self.onItemDoubleClick)
        
        layout.addWidget(self.tabla)
        self.refrescar()

    def onItemDoubleClick(self, item):
        row = item.row()
        uid = int(self.tabla.item(row, 0).text())
        u = next((x for x in self.ctrl.getUsuarios() if x.getId() == uid), None)
        if u:
            self.abrirFormulario(u)

    def refrescar(self):
        self.tabla.setRowCount(0)
        for i, u in enumerate(self.ctrl.getUsuarios()):
            self.tabla.insertRow(i)
            self.tabla.setItem(i, 0, QTableWidgetItem(str(u.getId())))
            self.tabla.setItem(i, 1, QTableWidgetItem(u.getNombre()))
            self.tabla.setItem(i, 2, QTableWidgetItem(u.getRol()))
            self.tabla.setItem(i, 3, QTableWidgetItem(u.getUsername()))
            self.tabla.setItem(i, 4, QTableWidgetItem(str(u.getEdad())))
            self.tabla.setItem(i, 5, QTableWidgetItem(u.getSexo()))

    def abrirFormulario(self, u):
        from Vista.UsuarioDialog import UsuarioDialog
        dlg = UsuarioDialog(self.window(), u, self.ctrl)
        dlg.exec()
        self.refrescar()
