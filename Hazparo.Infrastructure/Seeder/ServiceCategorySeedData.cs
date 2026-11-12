namespace Hazparo.Infrastructure.Identity;

public sealed class ServiceCategorySeedItem
{
    public ServiceCategorySeedItem(
        string groupName,
        string name,
        string slug,
        short sortOrder)
    {
        GroupName = groupName;
        Name = name;
        Slug = slug;
        SortOrder = sortOrder;
    }

    public string GroupName { get; }
    public string Name { get; }
    public string Slug { get; }
    public short SortOrder { get; }
}

public static class ServiceCategorySeedData
{
    public static IReadOnlyList<ServiceCategorySeedItem> All { get; } =
    [
        new("Casa", "Plomería", "casa-plomeria", 1),
        new("Casa", "Electricidad", "casa-electricidad", 2),
        new("Casa", "Cerrajería", "casa-cerrajeria", 3),
        new("Casa", "Destapacaños", "casa-destapacanos", 4),
        new("Casa", "Boilers y gas", "casa-boilers-y-gas", 5),
        new("Casa", "Aire acondicionado", "casa-aire-acondicionado", 6),
        new("Casa", "Línea blanca", "casa-linea-blanca", 7),
        new("Casa", "Herrería de seguridad", "casa-herreria-de-seguridad", 8),
        new("Casa", "Herrería decorativa", "casa-herreria-decorativa", 9),
        new("Casa", "Bombas e hidroneumáticos", "casa-bombas-e-hidroneumaticos", 10),
        new("Casa", "Impermeabilización", "casa-impermeabilizacion", 11),
        new("Casa", "Vidrio y aluminio", "casa-vidrio-y-aluminio", 12),
        new("Casa", "Tablaroca", "casa-tablaroca", 13),
        new("Casa", "Carpintería a medida", "casa-carpinteria-a-medida", 14),
        new("Casa", "Cisternas y tinacos", "casa-cisternas-y-tinacos", 15),
        new("Casa", "Fosas sépticas y desazolve", "casa-fosas-septicas-y-desazolve", 16),
        new("Casa", "Pintores", "casa-pintores", 17),
        new("Casa", "Albañilería", "casa-albanileria", 18),
        new("Casa", "Jardinería y poda", "casa-jardineria-y-poda", 19),
        new("Casa", "Limpieza doméstica", "casa-limpieza-domestica", 20),
        new("Casa", "Limpieza de muebles", "casa-limpieza-de-muebles", 21),
        new("Casa", "Pisos y azulejo", "casa-pisos-y-azulejo", 22),
        new("Casa", "Persianas y cortinas", "casa-persianas-y-cortinas", 23),
        new("Casa", "Cocinas integrales", "casa-cocinas-integrales", 24),
        new("Casa", "Mármol y granito", "casa-marmol-y-granito", 25),
        new("Casa", "Albercas", "casa-albercas", 26),
        new("Casa", "Portones automáticos", "casa-portones-automaticos", 27),
        new("Casa", "Escombro y demolición", "casa-escombro-y-demolicion", 28),
        new("Casa", "Mudanzas y fletes", "casa-mudanzas-y-fletes", 29),
        new("Casa", "Aparatos chicos", "casa-aparatos-chicos", 30),
        new("Casa", "Palapa y techo de huano", "casa-palapa-y-techo-de-huano", 31),

        new("Auto", "Mecánica a domicilio", "auto-mecanica-a-domicilio", 32),
        new("Auto", "Taller mecánico", "auto-taller-mecanico", 33),
        new("Auto", "Cerrajería automotriz", "auto-cerrajeria-automotriz", 34),
        new("Auto", "Hojalatería y pintura", "auto-hojalateria-y-pintura", 35),
        new("Auto", "Cristales y parabrisas", "auto-cristales-y-parabrisas", 36),
        new("Auto", "Audio y alarmas para auto", "auto-audio-y-alarmas", 37),
        new("Auto", "Detallado a domicilio", "auto-detallado-a-domicilio", 38),
        new("Auto", "Llanteras móviles", "auto-llanteras-moviles", 39),
        new("Auto", "Auxilio vial y grúas", "auto-auxilio-vial-y-gruas", 40),
        new("Auto", "Motos", "auto-motos", 41),
        new("Auto", "Bicicletas", "auto-bicicletas", 42),

        new("Seguridad y tecnología", "Cámaras CCTV", "seguridad-tecnologia-camaras-cctv", 43),
        new("Seguridad y tecnología", "Alarmas y control de acceso", "seguridad-tecnologia-alarmas-y-control-de-acceso", 44),
        new("Seguridad y tecnología", "Paneles solares", "seguridad-tecnologia-paneles-solares", 45),
        new("Seguridad y tecnología", "Redes y antenas", "seguridad-tecnologia-redes-y-antenas", 46),
        new("Seguridad y tecnología", "Cómputo y celulares", "seguridad-tecnologia-computo-y-celulares", 47),

        new("Negocios", "Trampas de grasa", "negocios-trampas-de-grasa", 48),

        new("Otros", "Fumigación", "otros-fumigacion", 49),
        new("Otros", "Soldadura autógena", "otros-soldadura-autogena", 50),
        new("Otros", "Rotulación", "otros-rotulacion", 51),
        new("Otros", "Afilado", "otros-afilado", 52)
    ];
}