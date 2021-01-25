Endast filerna under /font/ , ifrån: 
https://github.com/twbs/icons/tree/main/font

OBS! Jag har även döpt om .css filen till: .scss och lagt med den i: 
compilerconfig.json och där lagt till: 
```json
,
  {
    "outputFile": "./wwwroot/lib/bootstrap-icons/bootstrap-icons.css",
    "inputFile": "./wwwroot/lib/bootstrap-icons/bootstrap-icons.scss",
    "minify": {
      "enabled": true
    },
    "includeInProject": true,
    "options": {
      "sourceMap": true
    }
  }
```


Version 1.3.0 (hämtad 2021-01-20): 
https://github.com/twbs/icons/releases