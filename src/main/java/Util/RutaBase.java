package Util;

import java.io.File;

public class RutaBase {

    private static String raiz = null;

    public static String getRaiz() {
        if (raiz != null) return raiz;

        try {
            File jar = new File(RutaBase.class.getProtectionDomain()
                    .getCodeSource().getLocation().toURI());
            File carpeta = jar.isFile() ? jar.getParentFile() : jar;

            while (carpeta != null) {
                if (new File(carpeta, "Images").exists() ||
                    new File(carpeta, "pom.xml").exists()) {
                    raiz = carpeta.getAbsolutePath();
                    return raiz;
                }
                carpeta = carpeta.getParentFile();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        raiz = System.getProperty("user.dir");
        return raiz;
    }

    public static File getImages() {
        File carpeta = new File(getRaiz(), "Images");
        if (!carpeta.exists()) carpeta.mkdirs();
        return carpeta;
    }

    public static File getImagen(String nombre) {
        return new File(getImages(), nombre);
    }
}
