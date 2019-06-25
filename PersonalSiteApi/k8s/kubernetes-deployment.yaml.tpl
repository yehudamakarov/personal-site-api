apiVersion: apps/v1
kind: Deployment
metadata:
  name: personal-site-api
spec:
  replicas: 1
  selector:
    matchLabels:
      component: personal-site-api
  template:
    metadata:
      labels:
        component: personal-site-api
    spec:
      containers:
        - name: personal-site-api
          image: gcr.io/_CONTAINER_PROJECT_ID/REPO_NAME:SHORT_SHA
          ports:
            - containerPort: 5000