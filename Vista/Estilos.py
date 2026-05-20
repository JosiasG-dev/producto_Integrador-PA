from PyQt6.QtGui import QColor, QFont
from PyQt6.QtWidgets import QPushButton, QLineEdit
from PyQt6.QtCore import Qt

class Estilos:
    BG_OSCURO = QColor(24, 24, 27)
    BG_MUY_OSCURO = QColor(9, 9, 11)
    BG_CLARO = QColor(250, 250, 250)
    BG_BLANCO = QColor(255, 255, 255)
    BG_ZINC_100 = QColor(244, 244, 245)
    BG_ZINC_200 = QColor(228, 228, 231)

    INDIGO = QColor(79, 70, 229)
    INDIGO_OSCURO = QColor(67, 56, 202)
    INDIGO_CLARO = QColor(238, 242, 255)

    EMERALD = QColor(5, 150, 105)
    EMERALD_CLARO = QColor(236, 253, 245)

    ROSE = QColor(225, 29, 72)
    ROSE_CLARO = QColor(255, 241, 242)
    ROSE_OSCURO = QColor(190, 18, 60)

    AMBER = QColor(217, 119, 6)
    AMBER_CLARO = QColor(255, 251, 235)

    TEXTO_PRINCIPAL = QColor(24, 24, 27)
    TEXTO_SECUNDARIO = QColor(113, 113, 122)
    TEXTO_TENUE = QColor(161, 161, 170)
    TEXTO_BLANCO = QColor(255, 255, 255)

    BORDE = QColor(228, 228, 231)

    FUENTE_TITULO = QFont("SansSerif", 22, QFont.Weight.Bold)
    FUENTE_SUBTITULO = QFont("SansSerif", 14, QFont.Weight.Bold)
    FUENTE_NORMAL = QFont("SansSerif", 13, QFont.Weight.Normal)
    FUENTE_BOLD = QFont("SansSerif", 13, QFont.Weight.Bold)
    FUENTE_SMALL = QFont("SansSerif", 11, QFont.Weight.Bold)
    FUENTE_XS = QFont("SansSerif", 10, QFont.Weight.Bold)
    FUENTE_GRANDE = QFont("SansSerif", 28, QFont.Weight.Bold)
    FUENTE_MONO = QFont("Monospaced", 12, QFont.Weight.Bold)

    RADIO_BORDE = 16
    PADDING = 16
    PADDING_GRANDE = 24

    @staticmethod
    def botonPrimario(texto):
        btn = QPushButton(texto)
        btn.setFont(Estilos.FUENTE_XS)
        btn.setCursor(Qt.CursorShape.PointingHandCursor)
        btn.setStyleSheet(f"""
            QPushButton {{
                background-color: {Estilos.INDIGO.name()};
                color: {Estilos.TEXTO_BLANCO.name()};
                border-radius: 20px;
                border: none;
                padding: 10px;
            }}
            QPushButton:hover {{
                background-color: {Estilos.INDIGO_OSCURO.name()};
            }}
        """)
        return btn

    @staticmethod
    def botonPeligro(texto):
        btn = Estilos.botonPrimario(texto)
        btn.setStyleSheet(f"""
            QPushButton {{
                background-color: {Estilos.ROSE.name()};
                color: {Estilos.TEXTO_BLANCO.name()};
                border-radius: 20px;
                border: none;
                padding: 10px;
            }}
            QPushButton:hover {{
                background-color: {Estilos.ROSE_OSCURO.name()};
            }}
        """)
        return btn

    @staticmethod
    def botonSecundario(texto):
        btn = Estilos.botonPrimario(texto)
        btn.setStyleSheet(f"""
            QPushButton {{
                background-color: {Estilos.BG_ZINC_200.name()};
                color: {Estilos.TEXTO_PRINCIPAL.name()};
                border-radius: 20px;
                border: none;
                padding: 10px;
            }}
            QPushButton:hover {{
                background-color: #d4d4d8;
            }}
        """)
        return btn

    @staticmethod
    def botonExito(texto):
        btn = Estilos.botonPrimario(texto)
        btn.setStyleSheet(f"""
            QPushButton {{
                background-color: {Estilos.EMERALD.name()};
                color: {Estilos.TEXTO_BLANCO.name()};
                border-radius: 20px;
                border: none;
                padding: 10px;
            }}
            QPushButton:hover {{
                background-color: #047857;
            }}
        """)
        return btn

    @staticmethod
    def campo(placeholder=""):
        tf = QLineEdit()
        tf.setPlaceholderText(placeholder)
        tf.setFont(Estilos.FUENTE_BOLD)
        tf.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 10px 14px;
                border-radius: 4px;
            }}
            QLineEdit:focus {{
                border: 2px solid {Estilos.INDIGO.name()};
            }}
        """)
        return tf
