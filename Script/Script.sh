#!/bin/bash
cd ..

while [true]; do
  echo "This is the Project Assistant, the available commands are:
  run : Executes the Project
  report : Compiles and generates Project Report PDF
  show_report : Display Report PDF
  slides : Compliles and Generates project Presentation
  show_slides : Display Presentation PDF
  clean : Remove Auxiliary Files
  exit : Quit the program"

  echo "\n Type a Command"
  read command

  case $command in
  "run")
    #Run
    dotnet watch run --project MoogleServer
    ;;

  "report")
    #Report
    cd Informe
    pdflatex --shell-escape report.tex
    # Check if the pdf file is generated
    if [ -f "report.pdf" ]; then
      # Display a success message
      echo "PDF file generated: report.pdf"
    else
      # Display an error message
      echo "Error: PDF file not generated"
    fi
    cd ..
    ;;

  #ShowReport
  "show_report")
    cd Informe
    if [ -f "report.pdf" ]; then
      xdg-open "report.pdf"
    else
      pdflatex report.tex
      xdg-open "report.pdf"
    fi
    cd ..
    ;;

  #Slides
  "slides")
    cd Slides
    pdflatex --shell-escape slides.tex
    # Check if the pdf file is generated
    if [ -f "slides.pdf" ]; then
      # Display a success message
      echo "PDF file generated: slides.pdf"
    else
      # Display an error message
      echo "Error: PDF file not generated"
    fi
    cd ..
    ;;

  #ShowSlides
  "show_slides")
    cd ..
    cd Slides
    if [ -f "slides.pdf" ]; then
      xdg-open "slides.pdf"
    else
      pdflatex slides.tex
      xdg-open "slides.pdf"
    fi
    cd ..

    ;;
  #Clean
  "clean")
    cd Informe
    rm *.aux *.log *.out *.toc *.gz *.bbl *.blg *.pdfsync *.synctex.gz
    cd ..
    cd Slides
    rm *.aux *.log *.out *.toc *.gz *.bbl *.blg *.pdfsync *.synctex.gz
    cd ..
    ;;

  "exit")
    exit
    ;;

  *)
    echo "Uknown command, try again"
    ;;
  esac
done
