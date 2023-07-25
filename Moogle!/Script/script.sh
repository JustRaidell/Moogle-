#!/usr/bin/bash
echo Options:
echo run: Ejecuta el proyecto
echo report: Compila el informe
echo slides: Compila la presentación
echo show_report: Muestra el informe 
echo show_slides: Muestra la presentación
echo clean: Elimina los archivos auxiliares
echo finish: Sale de la consola 
echo " "


run () {
    cd ..
    make dev
}

report () {
    cd ..
    cd Informe
    pdflatex Informe.tex
}

show_report () {
    cd ..
    cd Informe
    if ! [ -f ./Informe.pdf ];
    then
        pdflatex Informe.tex
    fi
    evince Informe.pdf
}

slides () {
    cd ..
    cd Presentación
    pdflatex Presentación.tex
}

show_slides () {
    cd ..
    cd Presentación
    if ![ -f ./Presentación.pdf ];
    then
        pdflatex Presentación.tex
    fi
    evince Presentación.pdf
}

clean () {
    cd ..
    cd Informe
    if [ -f ./Informe.aux -a -f ./Informe.log ]
    then
        rm Informe.aux Informe.log
    fi
    cd ..
    cd Presentación
    if [ -f ./Presentación.aux -a -f ./Presentación.log ]
    then
        rm Presentación.aux Presentación.log
    fi
    cd ..
    cd MoogleEngine
    if [ -d ./bin -a -d ./obj ]
    then
        rm -r ./bin ./obj
    fi
    cd ..
    cd MoogleServer
    if [ -d ./bin -a -d ./obj ]
    then
        rm -r ./bin ./obj
    fi
    echo Archivos eliminados
} 

finish () { exit; }

while true
do
read selection

case $selection in
run)
run;;
report)
report;;
show_report)
show_report;;
slides)
slides;;
show_slides)
show_slides;;
clean)
clean;;
finish)
finish;;
*)
echo Opción inválida;;
esac

echo " "
done
