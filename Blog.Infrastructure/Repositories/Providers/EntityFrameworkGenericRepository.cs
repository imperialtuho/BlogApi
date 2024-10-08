﻿using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Blog.Domain.Exceptions;
using Blog.Domain.SharedKernel;
using MAG.Product.Application.Configurations.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories.Providers
{
    public abstract class EntityFrameworkGenericRepository<C, T> : IEntityFrameworkGenericRepository<T>
        where T : BaseEntity<string>
        where C : DbContext
    {
        protected C _dbContext;
        protected ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private UserSession? _UserSession;
        private int? TenantIdentify => _httpContextAccessor.GetTenantIdentify();
        public int? TenantId => LoginSession?.TenantId ?? TenantIdentify;

        public UserSession? LoginSession
        {
            get => _UserSession ?? _httpContextAccessor?.GetUserSession();
            set
            {
                _UserSession = value;
            }
        }

        protected EntityFrameworkGenericRepository(DbContextOptions<C> options, ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _dbContext = Activator.CreateInstance(typeof(C), options) as C ?? throw new InvalidOperationException("Cannot create DbContext");
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual async Task AddAndSaveChangesAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;
        }

        public async Task AddAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                InitializeEntity(entity);
            }

            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<T> GetEntityByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id) ?? throw new NotFoundException($"{nameof(GetEntityByIdAsync)} of {nameof(T)} with {id} not found!");
        }

        public async Task UpdateAndSaveChangesAsync(T entity)
        {
            UpdateEntity(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;
        }

        public async Task DeleteAndSaveChangesAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAndSaveChangesAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task<PaginatedResponse<T>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<T>, IQueryable<T>>? predicate = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

            if (predicate != null)
            {
                query = predicate(query);
            }

            return await PaginatedResponse<T>.CreateAsync(query, pageNumber, pageSize);
        }

        private void InitializeEntity(T entity)
        {
            entity.TenantId = TenantId;
            string author = string.Empty;

            if (string.IsNullOrEmpty(entity.CreatedBy))
            {
                author = LoginSession?.Email ?? "Site Administrators";
                entity.CreatedBy = author;
            }

            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = null;
            entity.ModifiedBy = author;
            entity.IsDeleted = false;
        }

        private void UpdateEntity(T entity)
        {
            string? author = LoginSession?.Email ?? "Site Administrators";
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = author;
        }
    }
}