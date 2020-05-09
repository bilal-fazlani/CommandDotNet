FROM squidfunk/mkdocs-material:5.0.1

COPY requirements-local.txt ./

RUN pip install -r requirements-local.txt