apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-deployment
spec:
  replicas: 1
  selector:
    matchLabels:
      component: api
  template:
    metadata:
      labels:
        component: api
    spec:
      containers:
        - name: api
          image: gcr.io/_CONTAINER_PROJECT_ID/REPO_NAME:SHORT_SHA
          ports:
            - containerPort: 5000