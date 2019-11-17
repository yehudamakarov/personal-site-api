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
      volumes:
        - name: google-application-credentials-file-mount
          secret:
            secretName: google-application-credentials-file
      containers:
        - name: personal-site-api
          image: gcr.io/_CONTAINER_PROJECT_ID/_REPO_NAME:_SHORT_SHA
          ports:
            - containerPort: 5000
          volumeMounts:
            - mountPath: /var/secret/google
              name: google-application-credentials-file-mount
          env:
            - name: ASPNETCORE_URLS
              value: "http://*:5000"

            - name: JWT_SIGNING_KEY
              valueFrom:
                secretKeyRef:
                  key: JWT_SIGNING_KEY
                  name: api-deployment-secret

            - name: JWT_TOKEN_EXPIRY_DURATION_IN_MINUTES
              value: '26'

            - name: ASPNETCORE_ENVIRONMENT
              value: _ASPNETCORE_ENVIRONMENT

            - name: GOOGLE_PROJECT_ID
              valueFrom:
                secretKeyRef:
                  name: api-deployment-secret
                  key: GOOGLE_PROJECT_ID

            - name: GOOGLE_APPLICATION_CREDENTIALS
              value: /var/secret/google/key.json

            - name: GITHUB_ACCESS_TOKEN
              valueFrom:
                secretKeyRef:
                  name: api-deployment-secret
                  key: GITHUB_ACCESS_TOKEN 
                