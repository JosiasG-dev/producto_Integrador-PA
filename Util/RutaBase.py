import os
import sys

class RutaBase:
    _raiz = None

    @staticmethod
    def getRaiz():
        if RutaBase._raiz is not None:
            return RutaBase._raiz
        try:
            if getattr(sys, 'frozen', False):
                carpeta = os.path.dirname(sys.executable)
            else:
                carpeta = os.path.dirname(os.path.abspath(__file__))
            while carpeta:
                if os.path.exists(os.path.join(carpeta, "Images")) or os.path.exists(os.path.join(carpeta, "requirements.txt")):
                    RutaBase._raiz = carpeta
                    return RutaBase._raiz
                parent = os.path.dirname(carpeta)
                if parent == carpeta:
                    break
                carpeta = parent
        except Exception as e:
            print(e)
        RutaBase._raiz = os.getcwd()
        return RutaBase._raiz

    @staticmethod
    def getImages():
        carpeta = os.path.join(RutaBase.getRaiz(), "Images")
        if not os.path.exists(carpeta):
            os.makedirs(carpeta)
        return carpeta

    @staticmethod
    def getImagen(nombre):
        return os.path.join(RutaBase.getImages(), nombre)
