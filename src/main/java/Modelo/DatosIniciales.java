package Modelo;

import java.util.ArrayList;
import java.util.List;

public class DatosIniciales {

	public static List<Producto> getProductos() {
		List<Producto> lista = new ArrayList<>();

		// DESPENSA BASICA (20)
		lista.add(new Producto("001","Arroz Verde Valle 1kg",25.00,60,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("002","Frijol Negro Isadora 500g",18.50,50,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("003","Azucar Zulka 1kg",32.00,45,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("004","Aceite Nutrioli 1L",48.00,40,"Despensa Basica","Litros / mL",""));
		lista.add(new Producto("005","Sal La Fina 1kg",12.00,55,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("006","Harina Maseca 1kg",28.00,50,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("007","Pasta Barilla Spaghetti 500g",22.00,45,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("008","Sopa Maruchan Camaron",8.50,80,"Despensa Basica","Piezas",""));
		lista.add(new Producto("009","Lentejas La Merced 500g",19.00,35,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("010","Avena Quaker 500g",38.00,40,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("011","Vinagre Clemente Jacques 500mL",18.00,30,"Despensa Basica","Litros / mL",""));
		lista.add(new Producto("012","Canela Entera McCormick 30g",15.00,25,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("013","Consome de Pollo Knorr 180g",24.00,50,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("014","Chile Guajillo 100g",12.00,35,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("015","Maicena 250g",16.00,30,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("016","Cafe Nescafe Clasico 135g",75.00,30,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("017","Te Lipton 25 sobres",32.00,25,"Despensa Basica","Piezas",""));
		lista.add(new Producto("018","Fideo Corto La Moderna 200g",10.00,60,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("019","Semola Tres Generaciones 400g",17.00,20,"Despensa Basica","Kilogramos / g",""));
		lista.add(new Producto("020","Polvo para Hornear Royal 80g",14.00,25,"Despensa Basica","Kilogramos / g",""));

		// LACTEOS Y HUEVO (15)
		lista.add(new Producto("021","Leche Lala Entera 1L",22.00,50,"Lacteos y Huevo","Litros / mL",""));
		lista.add(new Producto("022","Leche Alpura 1L",23.00,40,"Lacteos y Huevo","Litros / mL",""));
		lista.add(new Producto("023","Huevo San Juan 1kg",52.00,45,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("024","Crema Lala 200mL",22.00,30,"Lacteos y Huevo","Litros / mL",""));
		lista.add(new Producto("025","Queso Manchego Lala 400g",85.00,20,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("026","Yogurt Danone Natural 1kg",48.00,25,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("027","Mantequilla Lala 90g",28.00,30,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("028","Leche Condensada Nestle 397g",42.00,30,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("029","Queso Panela Covadonga 400g",72.00,15,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("030","Leche Evaporada Carnation 360mL",28.00,25,"Lacteos y Huevo","Litros / mL",""));
		lista.add(new Producto("031","Yogurt Yoplait Fresa 150g",18.00,35,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("032","Crema Acida Sello Rojo 250mL",19.00,20,"Lacteos y Huevo","Litros / mL",""));
		lista.add(new Producto("033","Jocoque Seco Lala 190g",22.00,15,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("034","Requeson Dos Pinos 250g",30.00,12,"Lacteos y Huevo","Kilogramos / g",""));
		lista.add(new Producto("035","Leche de Coco Goya 400mL",28.00,18,"Lacteos y Huevo","Litros / mL",""));

		// BEBIDAS Y LIQUIDOS (20)
		lista.add(new Producto("036","Coca-Cola 600mL",18.00,80,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("037","Pepsi 600mL",17.00,60,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("038","Agua Ciel 1L",12.00,100,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("039","Agua Epura 1.5L",15.00,80,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("040","Jugo Jumex Mango 473mL",18.00,50,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("041","Jugo Del Valle Naranja 450mL",18.00,45,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("042","Sprite 600mL",17.00,50,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("043","Fanta Naranja 600mL",17.00,45,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("044","Powerade Mora Azul 500mL",20.00,35,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("045","Gatorade Limon 600mL",20.00,35,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("046","Red Bull 250mL",38.00,25,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("047","Monster Energy 473mL",42.00,20,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("048","Sidral Mundet 600mL",16.00,40,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("049","Boing Guayaba 250mL",10.00,60,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("050","Lipton Te Limon 600mL",17.00,40,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("051","Nestea Durazno 500mL",17.00,35,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("052","Squirt 600mL",16.00,30,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("053","Sangria Senorial 600mL",14.00,30,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("054","Agua Mineral Penafiel 600mL",13.00,40,"Bebidas y Liquidos","Litros / mL",""));
		lista.add(new Producto("055","Horchata Pascual 500mL",15.00,25,"Bebidas y Liquidos","Litros / mL",""));

		// BOTANAS Y DULCES (15)
		lista.add(new Producto("056","Sabritas Original 45g",14.00,60,"Botanas y Dulces","Kilogramos / g",""));
		lista.add(new Producto("057","Ruffles 45g",14.00,55,"Botanas y Dulces","Kilogramos / g",""));
		lista.add(new Producto("058","Doritos Nacho 65g",16.00,50,"Botanas y Dulces","Kilogramos / g",""));
		lista.add(new Producto("059","Cheetos Flamin Hot 55g",14.00,45,"Botanas y Dulces","Kilogramos / g",""));
		lista.add(new Producto("060","Palomitas Popcorners 28g",15.00,30,"Botanas y Dulces","Kilogramos / g",""));
		lista.add(new Producto("061","Cacahuates Barcel 70g",15.00,40,"Botanas y Dulces","Kilogramos / g",""));
		lista.add(new Producto("062","Duvalin Vainilla-Fresa",8.00,60,"Botanas y Dulces","Piezas",""));
		lista.add(new Producto("063","Gansito Marinela",16.00,50,"Botanas y Dulces","Piezas",""));
		lista.add(new Producto("064","Pinguino Chocolate",14.00,45,"Botanas y Dulces","Piezas",""));
		lista.add(new Producto("065","Bubulubu",8.00,70,"Botanas y Dulces","Piezas",""));
		lista.add(new Producto("066","Paleta Payaso",6.00,80,"Botanas y Dulces","Piezas",""));
		lista.add(new Producto("067","Chicle Trident Menta 14 pzas",14.00,40,"Botanas y Dulces","Piezas",""));
		lista.add(new Producto("068","Chocolate Carlos V 18g",7.00,60,"Botanas y Dulces","Piezas",""));
		lista.add(new Producto("069","Mazapan De La Rosa",6.00,70,"Botanas y Dulces","Piezas",""));
		lista.add(new Producto("070","Takis Fuego 56g",16.00,55,"Botanas y Dulces","Kilogramos / g",""));

		// FRUTAS Y VERDURAS (10)
		lista.add(new Producto("071","Jitomate Saladet 1kg",28.00,30,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("072","Cebolla Blanca 1kg",22.00,35,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("073","Papa Blanca 1kg",18.00,40,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("074","Zanahoria 1kg",15.00,35,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("075","Chile Serrano 500g",20.00,25,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("076","Limon 1kg",25.00,30,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("077","Platano Tabasco 1kg",20.00,25,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("078","Manzana Roja 1kg",45.00,20,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("079","Ajo 100g",18.00,25,"Frutas y Verduras","Kilogramos / g",""));
		lista.add(new Producto("080","Epazote 100g",8.00,20,"Frutas y Verduras","Kilogramos / g",""));

		// CARNES Y SALCHICHONERIA (10)
		lista.add(new Producto("081","Jamon Salchicha FUD 500g",65.00,20,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("082","Salchicha Viena FUD 500g",55.00,20,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("083","Chorizo San Rafael 500g",70.00,15,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("084","Mortadela Campestre 200g",38.00,15,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("085","Tocino Rebanado Kir 200g",55.00,12,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("086","Pechuga de Pollo 1kg",105.00,15,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("087","Carne Molida Res 500g",85.00,12,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("088","Milanesa de Res 500g",95.00,10,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("089","Queso de Puerco 200g",32.00,15,"Carnes y Salchichoneria","Kilogramos / g",""));
		lista.add(new Producto("090","Spam 340g",55.00,18,"Carnes y Salchichoneria","Kilogramos / g",""));

		// ALIMENTOS PREPARADOS/ENLATADOS (15)
		lista.add(new Producto("091","Atun Herdez 140g",19.00,50,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("092","Sardina Clasica Caru 215g",22.00,40,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("093","Chile Chipotles La Costena 210g",22.00,35,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("094","Frijoles Refritos La Sierra 430g",28.00,40,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("095","Sopa de Tomate Campbells 305g",28.00,30,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("096","Maiz Dulce Caru 400g",22.00,35,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("097","Durazno en Almibar Del Monte 820g",42.00,25,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("098","Pasta de Tomate Del Fuerte 70g",10.00,50,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("099","Sopa Instantanea Nissin 85g",8.50,60,"Alimentos Preparados/Enlatados","Piezas",""));
		lista.add(new Producto("100","Chiles Jalapenos La Costena 220g",19.00,35,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("101","Mole Dona Maria 235g",42.00,20,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("102","Pozole Maseca 200g",25.00,25,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("103","Aceitunas La Fe 200g",35.00,18,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("104","Mermelada Smuckers Fresa 340g",45.00,20,"Alimentos Preparados/Enlatados","Kilogramos / g",""));
		lista.add(new Producto("105","Mantequilla de Cacahuate Jif 340g",65.00,15,"Alimentos Preparados/Enlatados","Kilogramos / g",""));

		// CUIDADO DEL HOGAR (25)
		lista.add(new Producto("106","Detergente Ariel 1kg",85.00,30,"Cuidado del Hogar","Kilogramos / g",""));
		lista.add(new Producto("107","Suavizante Downy 800mL",65.00,25,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("108","Jabon Roma 250g",14.00,50,"Cuidado del Hogar","Kilogramos / g",""));
		lista.add(new Producto("109","Jabon Zote Rosa 400g",18.00,40,"Cuidado del Hogar","Kilogramos / g",""));
		lista.add(new Producto("110","Cloro Cloralex 960mL",22.00,35,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("111","Pinol Limon 828mL",38.00,25,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("112","Fabuloso Lavanda 1L",35.00,30,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("113","Esponja Scotch Brite Verde",9.00,50,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("114","Bolsas Basura 10 pzas",22.00,40,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("115","Trapo Microfibra",18.00,30,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("116","Insecticida Raid 400mL",85.00,15,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("117","Papel de Bano Petalo 4 rollos",38.00,40,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("118","Servilletas Estrella 150 pzas",22.00,35,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("119","Papel Aluminio Wyandotte 10m",22.00,20,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("120","Papel Film Stretch 30cm x 20m",25.00,20,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("121","Jabon Palmolive Trastes 750mL",42.00,25,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("122","Quita Sarro Harpic 500mL",45.00,15,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("123","Limpia Vidrios Windex 500mL",42.00,15,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("124","Guantes de Latex Talla M",18.00,30,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("125","Detergente Salvo Liquido 1L",52.00,20,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("126","Suavitel Primavera 800mL",55.00,20,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("127","Blanqueador ACE 800mL",28.00,18,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("128","Cera Johnson 250mL",48.00,10,"Cuidado del Hogar","Litros / mL",""));
		lista.add(new Producto("129","Focos LED 9W paquete 2",55.00,20,"Cuidado del Hogar","Piezas",""));
		lista.add(new Producto("130","Pilas Duracell AA 4 pzas",48.00,25,"Cuidado del Hogar","Piezas",""));

		// HIGIENE Y CUIDADO PERSONAL (20)
		lista.add(new Producto("131","Shampoo Head and Shoulders 375mL",85.00,20,"Higiene y Cuidado Personal","Litros / mL",""));
		lista.add(new Producto("132","Acondicionador Pantene 375mL",75.00,18,"Higiene y Cuidado Personal","Litros / mL",""));
		lista.add(new Producto("133","Jabon Dove Original 135g",28.00,40,"Higiene y Cuidado Personal","Kilogramos / g",""));
		lista.add(new Producto("134","Desodorante Rexona Men 150mL",52.00,25,"Higiene y Cuidado Personal","Litros / mL",""));
		lista.add(new Producto("135","Pasta Dental Colgate 100mL",38.00,35,"Higiene y Cuidado Personal","Litros / mL",""));
		lista.add(new Producto("136","Cepillo Dental Oral-B Medio",28.00,30,"Higiene y Cuidado Personal","Piezas",""));
		lista.add(new Producto("137","Papel Higienico Scott 4 rollos",42.00,30,"Higiene y Cuidado Personal","Piezas",""));
		lista.add(new Producto("138","Crema Nivea 150mL",55.00,20,"Higiene y Cuidado Personal","Litros / mL",""));
		lista.add(new Producto("139","Rastrillo Gillette 2 pzas",35.00,20,"Higiene y Cuidado Personal","Piezas",""));
		lista.add(new Producto("140","Toallas Femeninas Kotex 10 pzas",35.00,25,"Higiene y Cuidado Personal","Piezas",""));
		lista.add(new Producto("141","Panuelos Kleenex 100 pzas",25.00,30,"Higiene y Cuidado Personal","Piezas",""));
		lista.add(new Producto("142","Gel Antibacterial Dettol 250mL",45.00,30,"Higiene y Cuidado Personal","Litros / mL",""));
		lista.add(new Producto("143","Hilo Dental Oral-B Menta 50m",22.00,20,"Higiene y Cuidado Personal","Piezas",""));
		lista.add(new Producto("144","Shampoo Sedal Ceramidas 350mL",72.00,18,"Higiene y Cuidado Personal","Litros / mL",""));
		lista.add(new Producto("145","Desodorante Lady Speed Stick 45g",38.00,20,"Higiene y Cuidado Personal","Kilogramos / g",""));
		lista.add(new Producto("146","Crema Ponds Clarant B3 200g",75.00,15,"Higiene y Cuidado Personal","Kilogramos / g",""));
		lista.add(new Producto("147","Protector Solar Banana Boat SPF50",98.00,12,"Higiene y Cuidado Personal","Kilogramos / g",""));
		lista.add(new Producto("148","Cotonetes Jumbo 100 pzas",18.00,25,"Higiene y Cuidado Personal","Piezas",""));
		lista.add(new Producto("149","Isodine Bucofaringeo 240mL",65.00,15,"Higiene y Cuidado Personal","Litros / mL",""));
		lista.add(new Producto("150","Crema Barba Gillette 175mL",52.00,18,"Higiene y Cuidado Personal","Litros / mL",""));

		return lista;
	}

	public static List<Usuario> getUsuarios() {
		List<Usuario> lista = new ArrayList<>();
		lista.add(new Usuario(1,"admin","admin123","ADMINISTRADOR","Director General",45,"Masculino"));
		lista.add(new Usuario(2,"cajero1","caja123","CAJERO","Juan Jose",28,"Masculino"));
		return lista;
	}

	public static List<Proveedor> getProveedores() {
		List<Proveedor> lista = new ArrayList<>();
		lista.add(new Proveedor(1,"GRUPO BIMBO","Juan Perez","8112345678","bimbo@proveedor.mx","Monterrey, NL",true));
		lista.add(new Proveedor(2,"COCA COLA FEMSA","Ana Torres","8187654321","femsa@proveedor.mx","Guadalupe, NL",true));
		lista.add(new Proveedor(3,"BARCEL S.A.","Carlos Ruiz","8199991111","barcel@proveedor.mx","CDMX",true));
		lista.add(new Proveedor(4,"LALA S.A. DE C.V.","Maria Lopez","8133334444","lala@proveedor.mx","Torreon, Coah",true));
		lista.add(new Proveedor(5,"NESTLE MEXICO","Pedro Hdz","5512345678","nestle@proveedor.mx","CDMX",true));
		lista.add(new Proveedor(6,"UNILEVER MEXICO","Sofia Ruiz","5598765432","unilever@prov.mx","Monterrey, NL",true));
		return lista;
	}

	public static String[] getCategorias() {
		return new String[] {
			"Despensa Basica","Lacteos y Huevo","Bebidas y Liquidos","Botanas y Dulces",
			"Frutas y Verduras","Carnes y Salchichoneria","Cuidado del Hogar",
			"Higiene y Cuidado Personal","Alimentos Preparados/Enlatados"
		};
	}

	public static String[] getUnidades() {
		return new String[] { "Piezas", "Kilogramos / g", "Litros / mL" };
	}
}
