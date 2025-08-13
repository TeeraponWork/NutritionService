using Application.Abstractions;
using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Entries
{
    public sealed record GetDailyEntriesQuery(DateOnly Date) : IRequest<IReadOnlyList<NutritionEntryDto>>;
    public sealed record GetDailySummaryQuery(DateOnly Date) : IRequest<DailySummaryDto>;

    public sealed class GetDailyEntriesHandler : IRequestHandler<GetDailyEntriesQuery, IReadOnlyList<NutritionEntryDto>>
    {
        private readonly INutritionDbContext _db;
        private readonly IUserContext _user;

        public GetDailyEntriesHandler(INutritionDbContext db, IUserContext user) { _db = db; _user = user; }

        public async Task<IReadOnlyList<NutritionEntryDto>> Handle(GetDailyEntriesQuery r, CancellationToken ct)
        {
            // join กับ Food เพื่อคำนวณโภชนาการกรณีอ้าง Food
            var q = from e in _db.Entries.AsNoTracking().Where(x => x.UserId == _user.UserId && x.Date == r.Date)
                    join f in _db.Foods.AsNoTracking() on e.FoodId equals f.Id into gf
                    from f in gf.DefaultIfEmpty()
                    select new { e, f };

            var list = await q.ToListAsync(ct);

            return list.Select(x =>
            {
                decimal cal, p, c, fat; decimal? fi, su, so;
                if (x.e.FoodId is not null && x.f is not null)
                {
                    var m = x.f.MacrosPerServing * (x.e.Quantity ?? 1);
                    (cal, p, c, fat, fi, su, so) = (m.Calories, m.Protein, m.Carbs, m.Fat, m.Fiber, m.Sugar, m.SodiumMg);
                }
                else
                {
                    var m = x.e.QuickAddMacros!;
                    (cal, p, c, fat, fi, su, so) = (m.Calories, m.Protein, m.Carbs, m.Fat, m.Fiber, m.Sugar, m.SodiumMg);
                }

                return new NutritionEntryDto(x.e.Id, x.e.MealType.ToString(), x.e.Date,
                    x.e.FoodId, x.e.Quantity,
                    cal, p, c, fat, fi, su, so, x.e.Notes);
            }).ToList();
        }
    }

    public sealed class GetDailySummaryHandler : IRequestHandler<GetDailySummaryQuery, DailySummaryDto>
    {
        private readonly INutritionDbContext _db;
        private readonly IUserContext _user;

        public GetDailySummaryHandler(INutritionDbContext db, IUserContext user)
        { _db = db; _user = user; }

        public async Task<DailySummaryDto> Handle(GetDailySummaryQuery r, CancellationToken ct)
        {
            var entries = await new GetDailyEntriesHandler(_db, _user).Handle(new(r.Date), ct);
            return new DailySummaryDto(
                r.Date,
                entries.Sum(x => x.Calories),
                entries.Sum(x => x.Protein),
                entries.Sum(x => x.Carbs),
                entries.Sum(x => x.Fat),
                entries.Sum(x => x.Fiber ?? 0) == 0 ? null : entries.Sum(x => x.Fiber ?? 0),
                entries.Sum(x => x.Sugar ?? 0) == 0 ? null : entries.Sum(x => x.Sugar ?? 0),
                entries.Sum(x => x.SodiumMg ?? 0) == 0 ? null : entries.Sum(x => x.SodiumMg ?? 0)
            );
        }
    }
}
